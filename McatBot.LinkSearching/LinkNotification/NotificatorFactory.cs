namespace McatBot.LinkSearching
{
    using Microsoft.Extensions.Options;

    public class NotificatorFactory
    {
        private readonly MessageLinkNotifier _messageLinkNotifier;
        private readonly PostCommentNotifier _postCommentNotifier;
        private readonly NotificatorFactoryConfig _config;

        public NotificatorFactory(MessageLinkNotifier messageLinkNotifier, PostCommentNotifier postCommentNotifier,
            IOptions<NotificatorFactoryConfig> config)
        {
            _messageLinkNotifier = messageLinkNotifier;
            _postCommentNotifier = postCommentNotifier;
            _config = config.Value;
        }

        public ILinkNotifier GetLinkNotifier()
        {
            if (_config.UseComment)
            {
                return _postCommentNotifier;
            }

            return _messageLinkNotifier;
        }
    }
}