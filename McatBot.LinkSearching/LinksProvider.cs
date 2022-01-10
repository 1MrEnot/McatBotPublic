namespace McatBot.LinkSearching
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Services;
    using Microsoft.Extensions.Logging;
    using Entities;
    using Microsoft.Extensions.Options;

    public class LinksProvider
    {
        private readonly IServiceLinkSearcher[] _linkSearchers;
        private readonly ILogger<LinksProvider> _logger;
        private readonly VkUrlShortener _vkUrlShortener;
        private readonly IOptions<LinkProviderConfig> _options;

        public LinksProvider(VkUrlShortener shortener, IOptions<LinkProviderConfig> config,
            IEnumerable<IServiceLinkSearcher> linkSearchers, ILogger<LinksProvider> logger)
        {
            _vkUrlShortener = shortener;
            _linkSearchers = linkSearchers.ToArray();
            _logger = logger;
            _options = config;
        }

        public async Task<string> GetLinksText(McatRelease release)
        {
            var results = new ConcurrentDictionary<string, string>();
            var shortenUrls = _options.Value.ShortenUrls;

            var usingLinkSearchers =
                _linkSearchers.Where(s => !_options.Value.IgnoreServices.Contains(s.GetMusicServiceName()));

            var tasks = usingLinkSearchers.Select(async s =>
            {
                var message = "Скоро будет...";
                var serviceName = s.GetMusicServiceName();
                try
                {
                    var url = await s.FindReleaseLink(release);
                    if (shortenUrls)
                        url = await _vkUrlShortener.Shorten(url);
                    message = url.ToShortString();
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Не удалось найти ссылку на {ReleaseName} в {ServiceName} : {Exception}",
                        release.Name, serviceName, e.ToString());
                }

                results[serviceName] = message;
            });

            await Task.WhenAll(tasks);

            var searchResult = results
                .Select(search => $"{search.Key} - {search.Value}").ToList();
            searchResult.Sort();

            return string.Join('\n', searchResult);
        }
    }
}