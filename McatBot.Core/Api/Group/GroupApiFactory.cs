namespace McatBot.Core.Api
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using VkNet.Model;

    public class GroupApiFactory
    {
        private readonly GroupApi _groupApi;
        private readonly GroupConfig _groupConfig;

        public GroupApiFactory(IOptions<GroupConfig> groupConfig)
        {
            groupConfig.CheckForNull(nameof(groupConfig));
            groupConfig.Value.CheckForNull(nameof(groupConfig.Value));

            _groupApi = new GroupApi();
            _groupConfig = groupConfig.Value;
        }

        public GroupApi GetGroupApi()
        {
            if (!_groupApi.IsAuthorized)
            {
                _groupApi.Authorize(new ApiAuthParams
                {
                    AccessToken = _groupConfig.Token
                });
            }

            return _groupApi;
        }

        public async Task<GroupApi> GetGroupApiAsync()
        {
            if (!_groupApi.IsAuthorized)
            {
                await _groupApi.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = _groupConfig.Token
                });
            }

            return _groupApi;
        }
    }
}