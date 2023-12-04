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
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

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
    public void UserDetails(Guid id)
    {
        
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ListUsers()
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpClient.BaseAddress = new Uri("http://localhost:8000/admin/realms/aspnet-core");

        // var response = await httpClient.GetStringAsync("http://localhost:8000/admin/realms/aspnet-core/users");
        var response = await httpClient.GetStringAsync($"{httpClient.BaseAddress}/users");

        Console.WriteLine(response);
        var deserializedData = JsonConvert.DeserializeObject<List<UserListViewModel>>(response); 
        
        return View(deserializedData ?? new List<UserListViewModel>());
    }
}
