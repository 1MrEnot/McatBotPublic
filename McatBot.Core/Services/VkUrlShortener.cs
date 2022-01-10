namespace McatBot.Core.Services
{
    using System;
    using System.Threading.Tasks;
    using Api;

    public class VkUrlShortener
    {
        private readonly UserApi _userApi;

        public VkUrlShortener(UserApi userApi)
        {
            _userApi = userApi;
        }

        public async Task<Uri> Shorten(Uri uri)
        {
            var link = await _userApi.Utils.GetShortLinkAsync(uri, true);
            return link.ShortUrl;
        }
    }
}