using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVCClient.Config;
using MVCClient.Helpers;
using MVCClient.Models;
using System.Net.Http.Headers;

namespace MVCClient.Controllers;

public class AccountController : Controller
{
    private readonly IdentityConfig _identityConfig;

    public AccountController(IOptions<IdentityConfig> identityConfiguration)
    {
        _identityConfig = identityConfiguration.Value;
    }

    public IActionResult Login()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> LogoutCallback()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> LoginCallback()
    {
        var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
        if (authResult?.Succeeded != true)
        {
            return RedirectToAction("Login");
        }

        var accessToken = authResult.Properties?.GetTokenValue("access_token");
        var refreshToken = authResult.Properties?.GetTokenValue("refresh_token");
        
        HttpContext.Session.SetString("access_token", accessToken);
        HttpContext.Session.SetString("refresh_token", refreshToken);
        
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ListUsers()
    {
        var exchange = new TokenExchange(_identityConfig);
        var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
        var token = await exchange.GetRefreshTokenAsync(refreshToken);
        var serviceAccessToken = await exchange.GetTokenExhangeAsync(token);

        using var httpClient = new HttpClient();
        //httpClient.BaseAddress = new Uri("http://localhost:8000/admin/realms/aspnet-core");
        if (!string.IsNullOrEmpty(serviceAccessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await httpClient.GetStringAsync("http://localhost:8000/admin/realms/aspnet-core/users");

        var result = new UserListViewModel();

        return View(result);
    }
}
