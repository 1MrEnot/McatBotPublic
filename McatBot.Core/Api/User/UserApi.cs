namespace McatBot.Core.Api
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using VkNet;
    using VkNet.Model;

    /// <summary>
    /// Предоставлят доступ к Api от имени пользователя
    /// </summary>
    public class UserApi : VkApi
    {
        public UserApi()
        {

        }

        public UserApi(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }

        public new long UserId { get; private set; }

        public new void Authorize(ApiAuthParams apiAuthParams)
        {
            base.Authorize(apiAuthParams);

            var users = Users.Get(Array.Empty<long>());
            UserId = users.Single().Id;
        }

        public async Task AuthorizeAsync(ApiAuthParams apiAuthParams)
        {
            await base.AuthorizeAsync(apiAuthParams);

            var users = await Users.GetAsync(Array.Empty<long>()).ConfigureAwait(false);
            UserId = users.Single().Id;
        }
    }
}