using System.Security.Claims;

namespace MVCClient.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }

    public static bool IsRealmAdmin(this ClaimsPrincipal user)
    {
        var result = user.IsAuthenticated() && user.Claims.Any(c => c is { Type: ClaimTypes.Role, Value: "r-admin" });
        return result;
        // return user.Claims.Any(c => c is { Type: ClaimTypes.Role, Value: "r-admin" });
    }
}
