namespace McatBot.LinkSearching
{
    using System;
    using System.Threading.Tasks;
    using Core.Services;
    using Entities;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Оповещает о найденных ссылках на релиз через сообщения
    /// </summary>
    public class MessageLinkNotifier : ILinkNotifier
    {
        private readonly GroupMessageSender _groupMessageSender;
        private readonly ILogger<MessageLinkNotifier> _logger;

        public MessageLinkNotifier(GroupMessageSender groupMessageSender, ILogger<MessageLinkNotifier> logger)
        {
            _groupMessageSender = groupMessageSender;
            _logger = logger;
        }

        public async Task NotifyAboutLinks(string text, McatPost mcatPost)
        {
            try
            {
                await _groupMessageSender.SendMessage(text);
                _logger.LogInformation("Выполнено оповещение о релизе в посте {PostId} через сообщения",
                    mcatPost.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось оповестить о релизе в посте {PostId} через сообщения", mcatPost.Id);
            }
        }
    }
}