using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCClient.Controllers;

public class AccountController : Controller
{
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
    public IActionResult ListUsers()
    {
        return View();
    }
}
