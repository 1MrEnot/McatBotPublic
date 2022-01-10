namespace McatBot.EventHandling
{
    using System.Threading.Tasks;
    using VkNet.Model.GroupUpdate;

    /// <summary>
    /// Обработчик события
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Обработка события
        /// </summary>
        /// <exception cref="EventHandlerException">При обработке события произошла ошибка</exception>
        Task<string> ProcessEvent(GroupUpdate updateEvent);
    }
}