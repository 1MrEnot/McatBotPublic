namespace McatBot.Entities
{
    using System;
    using System.Linq;

    public class McatRelease
    {
        public string Name { get; init; } = string.Empty;

        public string[] Authors { get; init; } = Array.Empty<string>();

        public string[] Feats { get; init; } = Array.Empty<string>();

        public string[] Remixers { get; init; } = Array.Empty<string>();

        public string[] Genres { get; init; } = Array.Empty<string>();

        public DateTime ReleaseDate { get; init; } = DateTime.MinValue;

        public Brand[] Brands { get; init; } = Array.Empty<Brand>();

        public override string ToString()
        {
            var res = $"{string.Join(", ", Authors)} - {Name}";

            if (Feats.Any())
                res += $" (feat. {string.Join(", ", Feats)})";

            if (Remixers.Any())
                res += $" ({string.Join(", ", Remixers)} remix)";

            return res;
        }
    }
}