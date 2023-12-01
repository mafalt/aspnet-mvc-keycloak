using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCClient.Models;

namespace MVCClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        var currentUser = HttpContext.User;
        _logger.LogInformation($"Current user name: {currentUser?.Identity?.Name ?? "Unknown user"}");

        foreach (var claim in currentUser?.Claims)
        {
            _logger.LogInformation($"---> Claim: {claim.Type} ({claim.Value})");
        }
        
        return View();
    }

    [HttpGet("/UserInfo")]
    [Authorize]
    public async Task<IActionResult> UserInformation()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}