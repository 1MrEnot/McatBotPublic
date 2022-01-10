namespace McatBot.Core.Api
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using VkNet.Model;

    public class UserApiFactory
    {
        private readonly UserApi _userApi;
        private readonly UserConfig _userConfig;

        public UserApiFactory(IOptions<UserConfig> userConfig)
        {
            userConfig.CheckForNull(nameof(userConfig));
            userConfig.Value.CheckForNull(nameof(userConfig.Value));

            _userApi = new UserApi();
            _userConfig = userConfig.Value;
        }

        public UserApi GetUserApi()
        {
            if (!_userApi.IsAuthorized)
            {
                _userApi.Authorize(new ApiAuthParams
                {
                    AccessToken = _userConfig.Token
                });
            }

            return _userApi;
        }

        public async Task<UserApi> GetUserApiAsync()
        {
            if (!_userApi.IsAuthorized)
            {
                await _userApi.AuthorizeAsync(new ApiAuthParams
                {
                    AccessToken = _userConfig.Token
                });
            }

            return _userApi;
        }
    }
}