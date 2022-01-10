namespace McatBot.LinkSearching
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Entities;
    using SoftThorn.MonstercatNet;

    public class MonstercatLinkSearcher : BaseServiceLinkSearcher
    {
        private readonly IMonstercatApi _api;

        public MonstercatLinkSearcher()
        {
            _api = MonstercatApi.Create(new HttpClient().UseMonstercatApiV2());
        }

        public override string GetMusicServiceName()
        {
            return "Monstercat";
        }


        public override async Task<Uri> FindReleaseLink(McatRelease mcatRelease)
        {
            try
            {
                var res = await _api.GetReleases(new ReleaseBrowseRequest
                {
                    Limit = 100
                }).ConfigureAwait(false);

                if (res.Results is null)
                {
                    throw new Exception("Не пришёл результат поиска с серверов Monstercat");
                }

                var release = res.Results.FirstOrDefault(r => IsRequestedTrack(r, mcatRelease));

                if (release is null)
                {
                    throw new Exception("Не найден трек");
                }

                return new Uri($"https://www.monstercat.com/release/{release.CatalogId}");
            }
            catch (Exception e)
            {
                throw new NoLinkException($"Не удалось найти в {GetMusicServiceName()} трек {mcatRelease.Name}", e);
            }
        }

        private static bool IsRequestedTrack(Release release, McatRelease searchingRelease)
        {
            var tokens = GetSearchTokens(searchingRelease);
            // var words = tokens.SelectMany(t => t.Split(' ')).ToArray();

            // var titleWords = release.Title.Split(' ');
            // var artistWords = release.ArtistsTitle.Split(' ');

            return tokens.Any(t => release.Title.Contains(t)) &&
                   tokens.Any(t => release.ArtistsTitle.Contains(t));
        }
    }
}