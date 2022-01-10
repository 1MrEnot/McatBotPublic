namespace McatBot.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Entities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class PostParser : IPostParser
    {
        // Release date: |01.01.2020|
        // Group 0 = digits/dots
        private readonly Regex _datetimePattern = new(@"[\d|.|-]+");

        private readonly string[] _datetimePatterns =
        {
            "dd.MM.yyyy",
            "dd.MM.yy",
            "dd,MM,yyyy",
            "dd,MM,yy",
            "yyyy-MM-dd"
        };

        // Genre:
        // Genres:
        // Style:
        // Styles:
        private readonly Regex _genresPattern = new(@"\b(?:[G|g]enre|[S|s]tyle)s?\b\s*:");

        private readonly ILogger<PostParser> _logger;
        /*
            Au5 - Snowblind (feat. Tasha Baxter) [Darren Styles Remix]
            Genre: Happy Hardcore
            Release Date: 14.12.2020
            Brand: #Uncaged@monstercat

            #Release@monstercat #Au5@monstercat #TashaBaxter@monstercat #DarrenStyles@monstercat
         */

        // |Au5| - |Snowblind| (|feat|. |Tasha Baxter|) [|Darren Styles Remix|]
        private readonly Regex _namePattern = new(@"\w([\w|\.]*\s*)*\w");
        private readonly PostParserConfig _options;

        public PostParser(ILogger<PostParser> logger, IOptions<PostParserConfig> config)
        {
            _logger = logger;
            _options = config.Value;
        }

        public bool TryParsePost(McatPost mcatPost, [NotNullWhen(true)] out McatRelease? release)
        {
            release = null;

            try
            {
                var clearText = ClearPostText(mcatPost.Text);

                var rows = clearText
                    .Split('\n')
                    .Select(r => r.Trim())
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .ToArray();

                if (!TrySplitByAuthorsAndName(rows[0], out var authorsPart, out var namePart))
                {
                    return false;
                }

                var authors = GetAuthors(authorsPart);

                namePart = ExtractInBracketsPart(namePart, "remix", out var remixersPart);
                var remixers = string.IsNullOrEmpty(remixersPart)
                    ? Array.Empty<string>()
                    : GetRemixes(remixersPart);

                namePart = ExtractInBracketsPart(namePart, "feat", out var featsPart);
                var feats = string.IsNullOrEmpty(featsPart)
                    ? Array.Empty<string>()
                    : GetFeats(featsPart);

                var genres = GetGenres(rows);

                var releaseDateRow = rows.Single(IsReleaseDate);
                var releaseDate = GetReleaseDate(releaseDateRow);

                release = new McatRelease
                {
                    Name = namePart,
                    Authors = authors.ToArray(),
                    Feats = feats,
                    Remixers = remixers,
                    Genres = genres,
                    ReleaseDate = releaseDate
                };

                return true;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e, "Не удалось распарсить пост");
                return false;
            }
        }

        private IEnumerable<string> GetAuthors(string authorsPart)
        {
            var authors = new List<string>();

            // Вытаскиваем имена, содержащие амперсанд или что-то типа того (\W, содержащийся в имени)
            foreach (var ampersandName in _options.AmpersandNames)
            {
                var nameStartIndex = authorsPart.IndexOf(ampersandName, StringComparison.InvariantCultureIgnoreCase);
                if (nameStartIndex == -1)
                {
                    continue;
                }

                authorsPart = authorsPart.Remove(nameStartIndex, nameStartIndex + ampersandName.Length);
                authors.Add(ampersandName);
            }

            // Заменяем все разделители, кроме запятых, на запятые
            foreach (var splitter in _options.Splitters)
            {
                var regex = new Regex(@$"\W{splitter}\W");
                authorsPart = regex.Replace(authorsPart, ",");
            }

            authors.AddRange(_namePattern
                .Matches(authorsPart)
                .Select(m => m.Value.Trim()));

            return authors;
        }

        private static string ClearPostText(string postText)
        {
            return Regex.Replace(postText, @"\[(club|id)\d*\|([\w| ]+)\]", @"$2");
        }

        private static bool TrySplitByAuthorsAndName(string title, out string authors, out string name)
        {
            authors = string.Empty;
            name = string.Empty;

            if (title.Count(s => s == '-') != 1)
            {
                return false;
            }

            var parts = title.Split('-');
            authors = parts[0].Trim();
            name = parts[1].Trim();

            return true;
        }

        private bool IsGenre(string row)
        {
            return _genresPattern.IsMatch(row);
        }

        private string[] GetGenres(IEnumerable<string> rows)
        {
            var arrRows = rows.ToArray();

            try
            {
                var row = arrRows.Single(IsGenre);

                var withoutPrefix = row.Split(':')[1].Trim();
                var words = withoutPrefix.Split(',').Select(w => w.Trim());
                return words.ToArray();
            }
            catch (Exception)
            {
                _logger.LogInformation("Не найдены жанры у релиза: {ReleaseText}",
                    string.Join('\n', arrRows));
                return Array.Empty<string>();
            }
        }

        private string[] GetFeats(string row)
        {
            var justAuthors = row.Replace("feat. ", string.Empty).Trim();
            return GetAuthors(justAuthors).ToArray();
        }

        private string[] GetRemixes(string row)
        {
            var justAuthors = row.Replace("Remix", string.Empty).Trim();
            return GetAuthors(justAuthors).ToArray();
        }

        private static bool IsReleaseDate(string row)
        {
            return row.StartsWith("release date:", StringComparison.InvariantCultureIgnoreCase);
        }

        private DateTime GetReleaseDate(string row)
        {
            var datetimePart = _datetimePattern.Match(row).Groups[0].Value;

            if (DateTime.TryParseExact(datetimePart, _datetimePatterns,
                CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var res))
            {
                return res;
            }

            throw new Exception($"Не получилось распарсить дату: {datetimePart}");
        }

        private static string ExtractInBracketsPart(string titleRow, string inBracketsToken, out string inBracketsPart)
        {
            titleRow = titleRow
                .Replace('[', '(')
                .Replace(']', ')');

            inBracketsPart = string.Empty;
            var index = titleRow.IndexOf(inBracketsToken, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                return titleRow;
            }

            var openBracketIndex = titleRow.LastIndexOf('(', index);
            var closeBracketIndex = titleRow.IndexOf(')', index);

            inBracketsPart = titleRow[openBracketIndex..(closeBracketIndex + 1)];
            titleRow = titleRow.Remove(openBracketIndex, closeBracketIndex - openBracketIndex + 1);

            return titleRow.Trim();
        }
    }
}