namespace McatBot.Core.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Entities;

    /// <summary>
    /// Преобразует пост на стене к релизу
    /// </summary>
    public interface IPostParser
    {
        /// <summary>
        /// Пытается преобразовать пост на стене к релизу
        /// </summary>
        /// <param name="mcatPost">Преобразуемый пост</param>
        /// <param name="release">Преобразованный релиз</param>
        /// <returns>Получилось ли преобразовать пост к релизу</returns>
        bool TryParsePost(McatPost mcatPost, [NotNullWhen(true)] out McatRelease? release);
    }
}