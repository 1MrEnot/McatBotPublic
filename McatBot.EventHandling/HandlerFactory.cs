namespace McatBot.EventHandling
{
    using VkNet.Enums.SafetyEnums;

    /// <summary>
    /// Фабрика обработчиков событий
    /// </summary>
    public class HandlerFactory
    {
        private readonly WallPostHandler _wallPostHandler;
        private readonly ConfirmationHandler _confirmationHandler;
        private readonly EmptyHandler _emptyHandler;

        public HandlerFactory(WallPostHandler wallPostHandler, ConfirmationHandler confirmationHandler)
        {
            _wallPostHandler = wallPostHandler;
            _confirmationHandler = confirmationHandler;
            _emptyHandler = new EmptyHandler();
        }

        /// <summary>
        /// Возвращает обработчик для указанного события
        /// </summary>
        public IEventHandler GetProcessor(GroupUpdateType eventType)
        {
            if (eventType == GroupUpdateType.WallPostNew)
            {
                return _wallPostHandler;
            }
            
            if (eventType == GroupUpdateType.Confirmation)
            {
                return _confirmationHandler;
            }

            return _emptyHandler;
        }
    }
}