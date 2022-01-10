namespace McatBot.LinkSearching
{
    using System;
    using Core;

    /// <summary>
    /// Не найдена ссылка на релиз
    /// </summary>
    public class NoLinkException : McatBotException
    {
        public NoLinkException()
        {
        }

        public NoLinkException(string message) : base(message)
        {
        }

        public NoLinkException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}