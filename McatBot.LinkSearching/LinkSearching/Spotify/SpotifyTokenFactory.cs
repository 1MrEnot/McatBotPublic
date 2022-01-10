namespace McatBot.LinkSearching.Spotify
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using RestSharp;

    /// <summary>
    /// Фабрика токенов для работы со Spotify
    /// </summary>
    public class SpotifyTokenFactory
    {
        private const string TokenUrl = "https://accounts.spotify.com/api/token";
        private readonly SpotifyConfig _config;

        public SpotifyTokenFactory(IOptions<SpotifyConfig> config)
        {
            config.CheckForNull(nameof(config));
            config.Value.CheckForNull(nameof(config.Value));

            _config = config.Value;
        }

        public string GetToken()
        {
            var client = new RestClient("https://accounts.spotify.com/api/token")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Basic {GetBase64AuthHeader()}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");

            var response = client.Execute(request);
            var responseValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            if (responseValues.TryGetValue("access_token", out var token))
            {
                return token;
            }

            throw new InvalidOperationException("Не удалось получить токен для Spotify");
        }

        public async Task<string> GetTokenAsync()
        {
            var client = new RestClient(TokenUrl)
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", $"Basic {GetBase64AuthHeader()}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);
            var responseValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

            if (responseValues.TryGetValue("access_token", out var token))
            {
                return token;
            }

            throw new InvalidOperationException("Не удалось получить токен для Spotify");
        }

        private string GetBase64AuthHeader()
        {
            var clientString = $"{_config.ClientId}:{_config.ClientSecret}";
            var bytes = Encoding.UTF8.GetBytes(clientString);
            return Convert.ToBase64String(bytes);
        }
    }
}