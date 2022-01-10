namespace McatBot.Tests
{
    using NUnit.Framework;
    using System;
    using Core.Services;
    using Entities;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;

    public class ParserTests
    {
        private PostParser _postParser;

        [SetUp]
        public void Setup()
        {
            _postParser = new PostParser(new NullLogger<PostParser>(), new OptionsWrapper<PostParserConfig>(new PostParserConfig()));
        }

        [Test]
        public void Test1()
        {
            const string text = @"
Au5, Fractal & Slippy - Snowblind (feat. Tasha Baxter, Rory & Danyka Nadeau) [Darren Styles, Stonebank & EMEL Remix]

Genre: Happy Hardcore, Hardstyle 
Release Date: 14.12.2020
Brand: #Uncaged@monstercat

#Release@monstercat #Au5@monstercat #TashaBaxter@monstercat #DarrenStyles@monstercat";

            var post = PostFromText(text);
            var parsingResult = _postParser.TryParsePost(post, out var release);

            Assert.True(parsingResult, "Не удалось распарсить пост");
            Assert.IsNotNull(release, "Релиз оказался null");

            Assert.AreEqual("Snowblind", release.Name);
            CollectionAssert.AreEqual(new [] {"Au5", "Fractal", "Slippy",}, release.Authors);
            CollectionAssert.AreEqual(new [] {"Happy Hardcore", "Hardstyle",}, release.Genres);
            CollectionAssert.AreEqual(new [] {"Tasha Baxter", "Rory", "Danyka Nadeau",}, release.Feats);
            CollectionAssert.AreEqual(new [] {"Darren Styles", "Stonebank", "EMEL",}, release.Remixers);
            Assert.AreEqual(new DateTime(2020, 12, 14), release.ReleaseDate);
        }

        [Test]
        public void Test2()
        {
            const string text = @"
Koven - Butterfly Effect (Acoustics)

Genre: Acoustic
Release Date: 04.05.2021
Brand: #Instinct@monstercat

Слушать в Spotify - vk.cc/c1BEi5 💿

#Release@monstercat #Koven@monstercat";

            var post = PostFromText(text);
            var parsingResult = _postParser.TryParsePost(post, out var release);

            Assert.True(parsingResult, "Не удалось распарсить пост");
            Assert.IsNotNull(release, "Релиз оказался null");

            Assert.AreEqual("Butterfly Effect (Acoustics)", release.Name);
            CollectionAssert.AreEqual(new [] {"Koven"}, release.Authors);
            CollectionAssert.AreEqual(new [] {"Acoustic"}, release.Genres);
            CollectionAssert.IsEmpty(release.Feats);
            CollectionAssert.IsEmpty(release.Remixers);
            Assert.AreEqual(new DateTime(2021, 05, 4), release.ReleaseDate);
        }

        private static McatPost PostFromText(string text) => new()
        {
            Text = text
        };
    }
}