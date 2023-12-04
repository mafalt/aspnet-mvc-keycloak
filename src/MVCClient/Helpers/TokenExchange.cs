using MVCClient.Config;
using Newtonsoft.Json;

namespace MVCClient.Helpers;

public class TokenExchange
{
    private readonly IdentityConfig _config;

    public TokenExchange(IdentityConfig config)
    {
        _config = config;
    }

    public async Task<string> GetRefreshTokenAsync(string refreshToken)
    {
        var url = $"{_config.AuthorityUrl}/realms/{_config.Realm}/{_config.TokenExchange}";
        var grantType = "refresh_token";
        var clientId = "admin-cli"; //_config.ClientId;
        var clientSecret = "gOoDBV9XKxNAxFR4nHuRqwDNvocKMCtZ"; // _config.Secret;
        var token = refreshToken;

        var form = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "clientId", clientId },
            { "client_secret", clientSecret },
            { "refresh_token", token },
        };

        try
        {
            using var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync(url, new FormUrlEncodedContent(form));
            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception(tokenResponse.Content.ReadAsStringAsync().Result);
            }

            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var receivedToken = JsonConvert.DeserializeObject<Token>(jsonContent);

            return receivedToken?.AccessToken ?? string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> GetTokenExhangeAsync(string accessToken)
    {
        var url = $"{_config.AuthorityUrl}/realms/{_config.Realm}/{_config.TokenExchange}";
        var grantType = "urn:ietf:params:oauth:grant-type:token-exchange";
        var clientId = "admin-cli"; //_config.ClientId;
        var clientSecret = "gOoDBV9XKxNAxFR4nHuRqwDNvocKMCtZ"; // _config.Secret;
        var audience = "mvc-client";

        var form = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "subject_token", accessToken },
            { "audience", audience },
        };

        try
        {
            using var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync(url, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            var receivedToken = JsonConvert.DeserializeObject<Token>(jsonContent);

            return receivedToken?.AccessToken ?? string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
