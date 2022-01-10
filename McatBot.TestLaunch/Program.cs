namespace McatBot.TestLaunch
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Core.Api;
    using Core.Services;
    using LinkSearching;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json;
    using VkNet.Model;
    using VkNet.Model.Attachments;
    using VkNet.Model.RequestParams;

    internal class Program
    {
        private const string Pattern = "https://vk.com/monstercat?w=wall-39531827_{0}";
        private static IHost _host;

        private static async Task Main(string[] args)
        {
            _host = LongPolling.Program.CreateHost();
            var provider = Create<LinksProvider>();
            var postParser = Create<PostParser>();

            var posts = await ReadPosts("allPosts.json");

            var cont = new ConcurrentBag<string>();
            foreach (var post in posts)
            {
                if (postParser.TryParsePost(post.ToMcatPost(), out var release))
                {
                    await provider.GetLinksText(release);
                }

                cont.Add(string.Format(Pattern, post.Id));
            }

            var fileWriter = new StreamWriter("res.txt", true, Encoding.UTF8)
            {
                AutoFlush = true
            };

            Console.WriteLine("Writing");
            foreach (var row in cont)
            {
                await fileWriter.WriteLineAsync(row);
            }
        }

        private static async Task<List<Post>> ReadPosts(string filename)
        {
            var text = await File.ReadAllTextAsync(filename);
            return JsonConvert.DeserializeObject<List<Post>>(text);
        }

        private static async Task SaveReleasesToFile(string filename)
        {
            const int count = 100;

            var api = await Create<UserApiFactory>().GetUserApiAsync();

            var allPosts = new List<Post>(512);

            var offset = 0;

            try
            {
                WallGetObject posts;
                do
                {
                    posts = await api.Wall.SearchAsync(new WallSearchParams
                    {
                        Count = count,
                        Offset = offset,
                        Query = "#Release",
                        Domain = "monstercat"
                    });
                    allPosts.AddRange(posts.WallPosts);
                    offset += count;
                    Console.WriteLine(offset);
                } while (posts.WallPosts.Any());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await using var file = File.CreateText(filename);
            var serializer = new JsonSerializer();
            serializer.Serialize(file, allPosts);
        }

        private static T Create<T>()
        {
            return ActivatorUtilities.CreateInstance<T>(_host.Services);
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            var aspnetEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{aspnetEnv}.json", true, true)
                .AddEnvironmentVariables();
        }
    }
}