using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using MVCClient.Config;

namespace MVCClient;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection("Identity"));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            })
            .AddOpenIdConnect(options =>
            {
                var config = new IdentityConfig();
                builder.Configuration.GetSection("Identity").Bind(config);

                options.Authority = $"{config.AuthorityUrl}/realms/{config.Realm}";
                options.ClientId = config.ClientId;
                options.ClientSecret = config.Secret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = config.CallbackPath;
                options.SignedOutCallbackPath = config.LogoutCallbackPath;
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("roles");
                options.RequireHttpsMetadata = builder.Environment.IsProduction();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = ClaimTypes.Role,
                };
            });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(name: "login-callback", pattern: "login-callback", defaults: new { controller = "Account", action = "LoginCallback" });
        app.MapControllerRoute(name: "logout-callback", pattern: "logout-callback", defaults: new { controller = "Account", action = "LogoutCallback" });
        app.MapDefaultControllerRoute();

        return app;
    }
}
