namespace McatBot.EventHandling
{
    using System;
    using Core;

    /// <summary>
    /// Исключение, произошедшее в результате работы обработчика событий
    /// </summary>
    public class EventHandlerException : McatBotException
    {
        public EventHandlerException()
        {
        }

        public EventHandlerException(string message) : base(message)
        {
        }

        public EventHandlerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}