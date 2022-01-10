namespace McatBot.EventHandling.ReleaseHandlers
{
    using System.Threading.Tasks;
    using Entities;
    using LinkSearching;

    public class ReleaseLinksWriter : IReleaseHandler
    {
        private readonly LinksProvider _linkProvider;
        private readonly ILinkNotifier _linkNotifier;

        public ReleaseLinksWriter(ILinkNotifier linkNotifier, LinksProvider linkProvider)
        {
            _linkNotifier = linkNotifier;
            _linkProvider = linkProvider;
        }

        public async Task HandleRelease(McatRelease release, McatPost mcatPost)
        {
            var message = await _linkProvider.GetLinksText(release);
            await _linkNotifier.NotifyAboutLinks(message, mcatPost);
        }
    }
}