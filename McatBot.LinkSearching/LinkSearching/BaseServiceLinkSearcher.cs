namespace McatBot.LinkSearching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities;

    public abstract class BaseServiceLinkSearcher : IServiceLinkSearcher
    {
        public abstract Task<Uri> FindReleaseLink(McatRelease mcatRelease);

        public abstract string GetMusicServiceName();

        /// <summary>
        /// Возвращает набор слов для поискового запроса указанного трека
        /// </summary>
        protected static List<string> GetSearchTokens(McatRelease mcatRelease)
        {
            var tokens = new List<string>();
            tokens.AddRange(mcatRelease.Authors);
            tokens.AddRange(mcatRelease.Feats);
            tokens.Add(mcatRelease.Name);

            return tokens;
        }

        /// <summary>
        /// Возвращает поисковый запроса для указанного трека
        /// </summary>
        protected static string GetSearchQuery(McatRelease mcatRelease)
        {
            return string.Join(" ", GetSearchTokens(mcatRelease));
        }
    }
}