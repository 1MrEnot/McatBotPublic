namespace McatBot.Core
{
    using System;

    public class McatBotException : Exception
    {
        public McatBotException()
        {
        }

        public McatBotException(string message) : base(message)
        {
        }

        public McatBotException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}