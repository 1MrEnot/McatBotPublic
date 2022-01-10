namespace McatBot.LinkSearching.Spotify
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Entities;
    using SpotifyAPI.Web;

    /// <summary>
    /// Searches for a album link in Spotify
    /// </summary>
    public class SpotifyServiceLinkSearcher : BaseServiceLinkSearcher
    {
        private readonly SpotifyTokenFactory _tokenFactory;
        private SpotifyClient _client;

        public SpotifyServiceLinkSearcher(SpotifyTokenFactory tokenFactory)
        {
            tokenFactory.CheckForNull(nameof(tokenFactory));

            _tokenFactory = tokenFactory;
            _client = new SpotifyClient(tokenFactory.GetToken());
        }

        public override string GetMusicServiceName()
        {
            return "Spotify";
        }

        public override async Task<Uri> FindReleaseLink(McatRelease mcatRelease)
        {
            var query = GetSearchQuery(mcatRelease);
            var searchRequest = new SearchRequest(SearchRequest.Types.All, query)
            {
                IncludeExternal = SearchRequest.IncludeExternals.Audio
            };
            var searchResponse = await GetSearchResponse(searchRequest);

            var album = TryGetAlbum(searchResponse);
            if (album is null)
            {
                throw new NoLinkException($"Не удалось найти в {GetMusicServiceName()} трек {mcatRelease}");
            }

            return new Uri(album.ExternalUrls["spotify"]);
        }

        private async Task<SearchResponse> GetSearchResponse(SearchRequest searchRequest)
        {
            SearchResponse searchResponse;

            try
            {
                searchResponse = await _client.Search.Item(searchRequest);
            }
            catch (APIUnauthorizedException)
            {
                await ResetClient();
                searchResponse = await _client.Search.Item(searchRequest);
            }
            catch (APITooManyRequestsException)
            {
                await Task.Delay(2000);
                searchResponse = await _client.Search.Item(searchRequest);
            }

            return searchResponse;
        }

        /// <summary>
        /// Returns album from search response. Returns null if there's no album
        /// </summary>
        /// <param name="response">Spotify search response</param>
        private static SimpleAlbum? TryGetAlbum(SearchResponse response)
        {
            var responseAlbum = response.Albums.Items?.FirstOrDefault();
            if (responseAlbum != null)
            {
                return responseAlbum;
            }

            var responseTrack = response.Tracks.Items?.FirstOrDefault();

            return responseTrack?.Album;
        }

        /// <summary>
        /// Sets new client with new token
        /// </summary>
        private async Task ResetClient()
        {
            var token = await _tokenFactory.GetTokenAsync().ConfigureAwait(false);
            _client = new SpotifyClient(token);
        }
    }
}