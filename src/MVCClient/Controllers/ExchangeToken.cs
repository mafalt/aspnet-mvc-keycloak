using MVCClient.Config;

namespace MVCClient.Controllers;
internal class ExchangeToken
{
    private IdentityConfig identityConfig;

    public ExchangeToken(IdentityConfig identityConfig)
    {
        this.identityConfig = identityConfig;
    }
}