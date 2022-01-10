namespace McatBot.LinkSearching
{
    using System;
    using System.Threading.Tasks;
    using Entities;

    /// <summary>
    /// Ищет релиз в стриминговом сервисе
    /// </summary>
    public interface IServiceLinkSearcher
    {
        /// <summary>
        /// Возвращает ссылку на релиз
        /// </summary>
        /// <exception cref="NoLinkException">Не удалось найти ссылку на релиз</exception>
        Task<Uri> FindReleaseLink(McatRelease mcatRelease);

        /// <summary>
        /// Возвращает название стримингого сервисва
        /// </summary>
        string GetMusicServiceName();
    }
}