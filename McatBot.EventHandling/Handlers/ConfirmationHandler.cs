namespace McatBot.EventHandling
{
    using System;
    using System.Threading.Tasks;
    using Core;
    using Core.Api;
    using VkNet.Model.GroupUpdate;

    /// <summary>
    /// Обработчик, подтверждающий работу сервера
    /// </summary>
    public class ConfirmationHandler : IEventHandler
    {
        private readonly UserApi _userApi;

        public ConfirmationHandler(UserApi userApi)
        {
            userApi.CheckForNull(nameof(userApi));
            _userApi = userApi;
        }

        public async Task<string> ProcessEvent(GroupUpdate updateEvent)
        {
            try
            {
                if (!updateEvent.GroupId.HasValue)
                    throw new Exception("Не пришёл id группы");

                var groupId = updateEvent.GroupId.Value;
                var code = await _userApi.Groups.GetCallbackConfirmationCodeAsync(groupId);
                return code;
            }
            catch (Exception e)
            {
                throw new EventHandlerException("Не удалось подтверить сервер", e);
            }
        }
    }
}
