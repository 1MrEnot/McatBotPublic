namespace McatBot.LinkSearching
{
    using System;

    public class LinkProviderConfig
    {
        public const string SectionName = "LinkProvider";

        public bool ShortenUrls { get; set; }

        public string[] IgnoreServices { get; set; } = Array.Empty<string>();
    }
}