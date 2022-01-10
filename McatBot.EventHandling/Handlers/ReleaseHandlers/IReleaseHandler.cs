namespace McatBot.EventHandling.ReleaseHandlers
{
    using System.Threading.Tasks;
    using Entities;

    public interface IReleaseHandler
    {
        Task HandleRelease(McatRelease release, McatPost mcatPost);
    }
}