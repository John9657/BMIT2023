using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BMIT2023.Models;

namespace BMIT2023.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Redirect to Billing Dashboard if user is logged in
        var userRole = TempData["UserRole"]?.ToString();
        if (!string.IsNullOrEmpty(userRole))
        {
            return RedirectToAction("Dashboard", "Billing");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
