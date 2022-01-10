namespace McatBot.EventHandling
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Services;
    using Microsoft.Extensions.Logging;
    using ReleaseHandlers;
    using VkNet.Model.GroupUpdate;

    /// <summary>
    /// Обработчик поста на стене
    /// </summary>
    public class WallPostHandler : IEventHandler
    {
        private readonly IPostParser _postParser;
        private readonly ILogger<WallPostHandler> _logger;
        private readonly IEnumerable<IReleaseHandler> _releaseHandlers;

        public WallPostHandler(IPostParser postParser, ILogger<WallPostHandler> logger,
            IEnumerable<IReleaseHandler> releaseHandlers)
        {
            _postParser = postParser;
            _logger = logger;
            _releaseHandlers = releaseHandlers;
        }

        public async Task<string> ProcessEvent(GroupUpdate updateEvent)
        {
            var mcatPost = updateEvent.WallPost.ToMcatPost();

            if (!_postParser.TryParsePost(mcatPost, out var mcatRelease))
            {
                return string.Empty;
            }

            var tasks = _releaseHandlers.Select(h => h.HandleRelease(mcatRelease, mcatPost));
            await Task.WhenAll(tasks);

            _logger.LogInformation("Обработан пост на стене с id = {PostId}", mcatPost.Id);

            return string.Empty;
        }
    }
}