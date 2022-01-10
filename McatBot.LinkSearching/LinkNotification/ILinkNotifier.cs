namespace McatBot.LinkSearching
{
    using System.Threading.Tasks;
    using Entities;

    /// <summary>
    /// Сообщает о найденных ссылках на релиз
    /// </summary>
    public interface ILinkNotifier
    {
        Task NotifyAboutLinks(string text, McatPost mcatPost);
    }
}