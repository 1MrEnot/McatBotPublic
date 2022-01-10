namespace McatBot.EventHandling
{
    using System.Threading.Tasks;
    using VkNet.Model.GroupUpdate;

    /// <summary>
    /// Обработчик по умолчанию - не делает ничего
    /// </summary>
    public class EmptyHandler : IEventHandler
    {
        public Task<string> ProcessEvent(GroupUpdate updateEvent)
        {
            return Task.FromResult(string.Empty);
        }
    }
}