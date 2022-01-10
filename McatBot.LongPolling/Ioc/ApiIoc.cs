namespace McatBot.LongPolling.Ioc
{
    using Core.Api;
    using Core.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ApiIoc
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddSingleton(sc =>
            {
                var userOptions = sc.GetRequiredService<IOptions<UserConfig>>();
                var userApiFactory = new UserApiFactory(userOptions);
                return userApiFactory.GetUserApi();
            });

            services.AddSingleton(sc =>
            {
                var groupOptions = sc.GetRequiredService<IOptions<GroupConfig>>();
                var groupApiFactory = new GroupApiFactory(groupOptions);
                return groupApiFactory.GetGroupApi();
            });

            services.AddTransient<GroupMessageSender>();

            services.AddOptions<GroupConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(GroupConfig.SectionName).Bind(settings);
                });
            services.AddOptions<UserConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(UserConfig.SectionName).Bind(settings);
                });

            return services;
        }
    }
}