namespace McatBot.LongPolling.Ioc
{
    using Core.Services;
    using LinkSearching;
    using LinkSearching.Spotify;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class LinkSearcherIoc
    {
        public static IServiceCollection AddLinkSearchers(this IServiceCollection services)
        {
            services.AddOptions<LinkProviderConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(LinkProviderConfig.SectionName).Bind(settings);
                });

            services.AddOptions<SpotifyConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(SpotifyConfig.SectionName).Bind(settings);
                });

            services.AddOptions<NotificatorFactoryConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(NotificatorFactoryConfig.SectionName).Bind(settings);
                });

            services.AddOptions<PostParserConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(PostParserConfig.SectionName).Bind(settings);
                });

            services.AddTransient<IPostParser, PostParser>();

            services.AddTransient<IServiceLinkSearcher, YandexServiceLinkSearcher>();
            services.AddTransient<IServiceLinkSearcher, SpotifyServiceLinkSearcher>();
            services.AddTransient<SpotifyTokenFactory>();

            services.AddTransient<LinksProvider>();
            services.AddTransient<VkUrlShortener>();

            services.AddTransient<PostCommentNotifier>();
            services.AddTransient<MessageLinkNotifier>();
            services.AddTransient<NotificatorFactory>();
            services.AddTransient(s =>
            {
                var factory = s.GetRequiredService<NotificatorFactory>();

                return factory.GetLinkNotifier();
            });

            return services;
        }
    }
}