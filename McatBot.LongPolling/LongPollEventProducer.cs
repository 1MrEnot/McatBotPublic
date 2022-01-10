namespace McatBot.LongPolling
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Api;
    using Microsoft.Extensions.Logging;
    using VkNet.Exception;
    using VkNet.Model;
    using VkNet.Model.GroupUpdate;
    using VkNet.Model.RequestParams;

    /// <summary>
    /// Получатель событий от ВК
    /// </summary>
    public class LongPollEventProducer
    {
        private readonly UserApi _userApi;
        private readonly GroupApi _groupApi;
        private readonly ILogger<LongPollEventProducer> _logger;

        private readonly TimeSpan _noChangesDelay = TimeSpan.FromSeconds(15);
        private LongPollServerResponse? _sessionInfo;
        private string? _ts;

        /// <summary>
        /// Были ли получены изменения в прошлый раз
        /// </summary>
        private bool _hadNoChanges;

        public LongPollEventProducer(UserApi userApi, GroupApi groupApi, ILogger<LongPollEventProducer> logger)
        {
            _userApi = userApi;
            _groupApi = groupApi;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список событий с момента последнего вызова метода
        /// </summary>
        public async Task<GroupUpdate[]> GetUpdates()
        {
            try
            {
                if (_sessionInfo is null)
                    await ResetSession();

                if (_hadNoChanges)
                    await Task.Delay(_noChangesDelay);

                return await GetHistory();
            }
            catch (LongPollKeyExpiredException)
            {
                _logger.LogTrace("Время действия  ключа LongPoll сессии истекло, обновление");
                await ResetSession();
                return await GetHistory();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось получить события группы");
                return Array.Empty<GroupUpdate>();
            }
        }

        private async Task ResetSession()
        {
            var groupId = Convert.ToUInt64(_groupApi.GroupId);
            _sessionInfo = await _userApi.Groups.GetLongPollServerAsync(groupId);
            _ts = _sessionInfo.Ts;
        }

        private async Task<GroupUpdate[]> GetHistory()
        {
            var updateResponse = await _userApi.Groups.GetBotsLongPollHistoryAsync(new BotsLongPollHistoryParams
            {
                Key = _sessionInfo?.Key,
                Server = _sessionInfo?.Server,
                Ts = _ts,
                Wait = 25
            });
            _ts = updateResponse.Ts;

            var updates = updateResponse.Updates.ToArray();
            _hadNoChanges = updates.Length == 0;

            return updates;
        }
    }
}