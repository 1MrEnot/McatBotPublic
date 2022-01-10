namespace McatBot.Core.Api
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using VkNet;
    using VkNet.Enums.Filters;
    using VkNet.Model;

    /// <summary>
    /// Предоставлят доступ к Api от группы
    /// </summary>
    public class GroupApi : VkApi
    {
        public GroupApi()
        {
        }

        public GroupApi(IServiceCollection serviceCollection) : base(serviceCollection)
        {
        }

        public long GroupId => Group.Id;

        public Group Group { get; private set; } = null!;

        public new void Authorize(ApiAuthParams apiAuthParams)
        {
            base.Authorize(apiAuthParams);
            var groups = Groups
                .GetById(Array.Empty<string>(), "", GroupsFields.CityId);

            Group = groups.Single();
        }

        public async Task AuthorizeAsync(ApiAuthParams apiAuthParams)
        {
            await base.AuthorizeAsync(apiAuthParams);
            var groups = await Groups
                .GetByIdAsync(Array.Empty<string>(), "", GroupsFields.CityId)
                .ConfigureAwait(false);

            Group = groups.Single();
        }
    }
}