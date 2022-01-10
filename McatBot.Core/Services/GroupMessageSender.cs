namespace McatBot.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Api;
    using Microsoft.Extensions.Logging;
    using VkNet.Model.RequestParams;

    /// <summary>
    /// Отправляет сообщения пользователям от имени группы
    /// </summary>
    public class GroupMessageSender
    {
        private static readonly Random Random = new();

        private readonly GroupApi _groupApi;
        private readonly UserApi _userApi;
        private readonly ILogger<GroupMessageSender> _logger;

        public GroupMessageSender(GroupApi groupApi, UserApi userApi, ILogger<GroupMessageSender> logger)
        {
            _groupApi = groupApi;
            _userApi = userApi;
            _logger = logger;
        }

        public async Task SendMessage(string message, long userId)
        {
            try
            {
                await _groupApi.Messages.SendAsync(new MessagesSendParams
                {
                    PeerId = userId,
                    Message = message,
                    RandomId = Random.Next()
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При отправке сообщения {Message} пользователю с Id = {Id} произошла ошибка",
                    message, userId);
            }
        }

        public Task SendMessage(string message)
        {
            return SendMessage(message, _userApi.UserId);
        }
    }
}