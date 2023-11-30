using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MVCClient;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

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
                options.Authority = "http://localhost:8080/realms/playaround";
                options.ClientId = "test";
                options.ClientSecret = "D9ZUqKDS8m0owhSVpemG3w2z5y5cqG2O";
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = "/login-callback";
                // options.CallbackPath = "/signin-oidc";
                // options.SignedOutCallbackPath = "/signout-callback-oidc";
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

                options.Events = new OpenIdConnectEvents()
                {
                    OnAuthorizationCodeReceived = ctx =>
                    {
                        Console.WriteLine("---> OnAuthorizationCodeReceived");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = ctx =>
                    {
                        Console.WriteLine("---> OnMessageReceived");
                        Console.WriteLine($"----> Token: {ctx.Token}");

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        Console.WriteLine("---> OnTokenValidated");
                        Console.WriteLine($"----> Token: {ctx.SecurityToken.UnsafeToString()}");
                        return Task.CompletedTask;
                    }
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

        // app.MapControllerRoute(
        //     name: "default",
        //     pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapPost("/signin-oidc", async context =>
        {
            await context.Response.WriteAsync("Authentication successful");
        });

        app.MapPost("/signout-callback-oidc", async context =>
        {
            await context.Response.WriteAsync("Sign-out successful");
        });

        app.UseEndpoints(endpoints =>
        {
            // Add endpoint for Keycloak authentication callback
            endpoints.MapControllerRoute(
                name: "login-callback",
                pattern: "login-callback",
                defaults: new { controller = "Account", action = "LoginCallback" });

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        return app;
    }
}
