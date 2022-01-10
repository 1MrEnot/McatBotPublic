namespace McatBot.LinkSearching
{
    using System;
    using System.Threading.Tasks;
    using Core.Api;
    using Entities;
    using Microsoft.Extensions.Logging;
    using VkNet.Model.RequestParams;

    /// <summary>
    /// Оповещает о найденных ссылках на релиз через комментарии к посту от имени группы
    /// </summary>
    public class PostCommentNotifier : ILinkNotifier
    {
        private readonly GroupApi _groupApi;
        private readonly ILogger<PostCommentNotifier> _logger;

        public PostCommentNotifier(GroupApi groupApi, ILogger<PostCommentNotifier> logger)
        {
            _groupApi = groupApi;
            _logger = logger;
        }

        public async Task NotifyAboutLinks(string text, McatPost mcatPost)
        {
            try
            {
                await _groupApi.Wall.CreateCommentAsync(new WallCreateCommentParams
                {
                    PostId = mcatPost.Id,
                    Message = text,
                    OwnerId = -_groupApi.GroupId,
                    FromGroup = _groupApi.GroupId
                });

                _logger.LogInformation("Выполнено оповещение о релизе в посте {PostId} через коммент к посту",
                    mcatPost.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось отправить сообщение к посту {PostId} от имени группы {GroupId}",
                    mcatPost.Id, _groupApi.GroupId);
            }
        }
    }
}