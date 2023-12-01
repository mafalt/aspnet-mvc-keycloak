namespace MVCClient.Config;

public class IdentityConfig
{
    public string ClientId { get; set; } = String.Empty;
    public string Secret { get; set; } = String.Empty;
    public string CallbackPath { get; set; } = String.Empty;
    public string AuthorityUrl { get; set; } = String.Empty;
    public string LogoutCallbackPath { get; set; } = String.Empty;
    public string TokenExchange { get; set; } = "protocol/openid-connect/token";
    public string Realm { get; set; } = string.Empty;
}
