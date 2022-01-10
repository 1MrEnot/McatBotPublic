namespace McatBot.LongPolling.Ioc
{
    using Core.Services;
    using EventHandling;
    using EventHandling.ReleaseHandlers;
    using Microsoft.Extensions.DependencyInjection;

    public static class EventHandlersIoc
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddTransient<WallPostHandler>();
            services.AddTransient<LinkGenerator>();
            services.AddTransient<IReleaseHandler, ReleaseLinksWriter>();
            services.AddTransient<IReleaseHandler, ReleaseDateChecker>();

            services.AddTransient<ConfirmationHandler>();

            services.AddTransient<HandlerFactory>();

            return services;
        }
    }
}