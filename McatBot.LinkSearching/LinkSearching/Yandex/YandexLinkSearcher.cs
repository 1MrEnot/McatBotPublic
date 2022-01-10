namespace McatBot.LinkSearching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Entities;
    using YandexMusicResolver;
    using YandexMusicResolver.AudioItems;
    using YandexMusicResolver.Config;

    public class YandexServiceLinkSearcher : BaseServiceLinkSearcher
    {
        private readonly Regex _albumLinkRegex = new(@"https?:\/\/music.yandex.ru\/album\/\d*");
        private readonly YandexMusicMainResolver _yandexMusicMainResolver;

        public YandexServiceLinkSearcher()
        {
            _yandexMusicMainResolver = new YandexMusicMainResolver(new EmptyYandexConfig());
        }

        public override string GetMusicServiceName()
        {
            return "Yandex Music";
        }

        public override async Task<Uri> FindReleaseLink(McatRelease mcatRelease)
        {
            var searchQuery = GetSearchQuery(mcatRelease);

            try
            {
                var results = await _yandexMusicMainResolver
                    .ResolveQuery(searchQuery)
                    .ConfigureAwait(false);

                if (results == null)
                {
                    throw new Exception("Не пришли результаты от яндекса");
                }

                if (results.Tracks == null || !results.Tracks.Any())
                {
                    throw new Exception("Не нашлось ни треков ни альбомов");
                }

                // var track = results.Tracks.First();
                var track = GetEqualRelease(mcatRelease, results.Tracks);

                var uri = track.Uri;
                var match = _albumLinkRegex.Match(uri);
                if (!match.Success)
                {
                    throw new Exception($"Не подходящий формат ссылки у Яндкеса: {uri}");
                }

                return new Uri(match.Value);
            }

            catch (Exception e)
            {
                throw new NoLinkException($"Не удалось найти в {GetMusicServiceName()} трек {mcatRelease}", e);
            }
        }

        private static YandexMusicTrack GetEqualRelease(McatRelease mcatRelease,
            IEnumerable<YandexMusicTrack> fondTracks)
        {
            var fondTracksArray = fondTracks.ToArray();
            var possibilities = fondTracksArray.Select(t =>
                    GetEqualityPossibility(mcatRelease, t))
                .ToArray();

            var equalTracks = fondTracksArray
                .Zip(possibilities)
                .Where(p => Math.Abs(p.Second - 1) < 0.01).ToArray();

            if (equalTracks.Length == 0)
            {
                throw new Exception($"Не найдено подходящих треков для релиза {mcatRelease}");
            }

            if (equalTracks.Length > 1)
            {
                var tracks = equalTracks.Select(t => t.First.Uri);
                throw new Exception(
                    $"Найдено больше одного подходящего трека для релиза {mcatRelease}: {string.Join('\n', tracks)}");
            }

            return equalTracks.Single().First;
        }

        /// <summary>
        /// Сравнивает на похожесть релиз с найденым треком
        /// </summary>
        /// <returns>Вероятнось того, что треки совпадают от 0 до 1</returns>
        private static double GetEqualityPossibility(McatRelease mcatRelease, YandexMusicTrack fondTrack)
        {
            var releaseTokens = GetSearchTokens(mcatRelease)
                .SelectMany(t => t.Split(' '))
                .ToArray();
            var yandexTokens = GetYandexTrackTokens(fondTrack).ToArray();

            var missingTokens = releaseTokens.Except(yandexTokens).ToArray();
            var extraTokens = yandexTokens.Except(releaseTokens).ToArray();

            return 1 - (float) missingTokens.Length / releaseTokens.Length *
                ((float) extraTokens.Length / yandexTokens.Length);
        }

        private static IEnumerable<string> GetYandexTrackTokens(YandexMusicTrack track)
        {
            var titleTokens = track.Title.Split(' ');
            var authorTokens = track.Author.Split(' ');

            return titleTokens.Concat(authorTokens);
        }
    }
}