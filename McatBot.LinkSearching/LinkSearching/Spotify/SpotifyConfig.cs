namespace McatBot.LinkSearching.Spotify
{
    public class SpotifyConfig
    {
        public const string SectionName = "Spotify";

        public string ClientId { get; set; } = null!;

        public string ClientSecret { get; set; } = null!;
    }
}