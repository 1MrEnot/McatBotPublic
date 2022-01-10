namespace McatBot.Core
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using VkNet.Model.Attachments;

    public static class Extensions
    {
        public static T CheckForNull<T>(this T argument, string name = "Object")
        {
            if (argument is null)
            {
                throw new ArgumentNullException($"{name} is null");
            }
            return argument;
        }

        public static IEnumerable<T> AsEnumerable<T>(this T element)
        {
            yield return element;
        }

        public static string ToShortString(this Uri uri)
        {
            return uri.Host + uri.AbsolutePath;
        }


        public static McatPost ToMcatPost(this Post post)
        {
            if (!post.Id.HasValue)
                throw new Exception("В посте не указан id");

            if (!post.Date.HasValue)
                throw new Exception("В посте не указана дата");

            return new McatPost
            {
                Id = post.Id.Value,
                Text = post.Text,
                PostDateTime = post.Date.Value
            };
        }
    }
}