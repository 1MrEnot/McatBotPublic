namespace McatBot.Core.Services
{
    using System;

    public class PostParserConfig
    {
        public const string SectionName = "PostParser";

        public string[] AmpersandNames { get; set; } = Array.Empty<string>();

        public string[] Splitters { get; set; } = Array.Empty<string>();
    }
}