using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyVacationController.Models;

namespace MyVacationController.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.HasClaim(claim => claim.Type == "IsAdmin"))
        {
            return RedirectToAction("Monthly", "Leave");
        }

        return RedirectToAction("Index", "Leave");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
