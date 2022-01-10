namespace McatBot.Core.Services
{
    using Api;

    public class LinkGenerator
    {
        private readonly GroupApi _groupApi;

        public LinkGenerator(GroupApi groupApi)
        {
            _groupApi = groupApi;
        }

        public string GetWallLinkToPost(long postId)
        {
            return $"https://vk.com/public{GroupId}?w=wall-{GroupId}_{postId}";
        }

        private long GroupId => _groupApi.GroupId;
    }
}