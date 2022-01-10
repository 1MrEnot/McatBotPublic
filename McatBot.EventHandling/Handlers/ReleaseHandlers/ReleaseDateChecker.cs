namespace McatBot.EventHandling.ReleaseHandlers
{
    using System;
    using System.Threading.Tasks;
    using Core.Services;
    using Entities;

    public class ReleaseDateChecker : IReleaseHandler
    {
        private readonly GroupMessageSender _groupMessageSender;
        private readonly LinkGenerator _linkGenerator;

        public ReleaseDateChecker(GroupMessageSender groupMessageSender, LinkGenerator linkGenerator)
        {
            _groupMessageSender = groupMessageSender;
            _linkGenerator = linkGenerator;
        }

        public Task HandleRelease(McatRelease release, McatPost mcatPost)
        {
            return release.ReleaseDate.Date == DateTime.Today
                ? Task.CompletedTask
                : _groupMessageSender.SendMessage(
                    $"У релиза, возможно, стоит неверная дата: {_linkGenerator.GetWallLinkToPost(mcatPost.Id)}");
        }
    }
}