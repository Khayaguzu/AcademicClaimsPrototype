using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AcademicClaimsPrototype.Models;

namespace AcademicClaimsPrototype.Controllers;

// HomeController handles general site pages like Index, Privacy, and Error
public class HomeController : Controller
{
    // Logger instance for logging information, warnings, or errors
    private readonly ILogger<HomeController> _logger;

    // Constructor to inject the logger dependency
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Displays the home page (Index view)
    public IActionResult Index()
    {
        return View();
    }

    // Displays the Privacy page
    public IActionResult Privacy()
    {
        return View();
    }

    // Displays the Error page when an unhandled exception occurs
    // ResponseCache attributes prevent caching of the error page
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Passes an ErrorViewModel to the view containing the request ID for troubleshooting
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
