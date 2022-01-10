namespace McatBot.LongPolling
{
    using System;
    using System.Threading.Tasks;
    using Core.Api;
    using EventHandling;
    using Microsoft.Extensions.Logging;

    public class LongPollApp
    {
        private readonly GroupApi _groupApi;
        private readonly HandlerFactory _handlerFactory;
        private readonly ILogger<LongPollApp> _logger;
        private readonly LongPollEventProducer _longPollEventProducer;

        public LongPollApp(HandlerFactory handlerFactory, LongPollEventProducer eventProducer,
            ILogger<LongPollApp> logger, GroupApi groupApi)
        {
            _handlerFactory = handlerFactory;
            _longPollEventProducer = eventProducer;
            _logger = logger;
            _groupApi = groupApi;
        }

        public async Task Run()
        {
            _logger.LogInformation("Бот запущен в группе {GroupName}", _groupApi.Group.Name);

            while (true)
            {
                var updates = await _longPollEventProducer.GetUpdates();
                foreach (var update in updates)
                {
                    var handler = _handlerFactory.GetProcessor(update.Type);

                    try
                    {
                        await handler.ProcessEvent(update);
                        _logger.LogInformation("Обработано событие типа {@UpdateType}", update.Type);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Не удалось обработать событие типа {@UpdateType}", update.Type);
                    }
                }
            }
        }
    }
}