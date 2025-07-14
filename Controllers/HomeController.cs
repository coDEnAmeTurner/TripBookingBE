using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TripBookingBE.Models;

namespace TripBookingBE.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMyDependency _myDependency;

    public HomeController(ILogger<HomeController> logger, IMyDependency myDependency)
    {
        _logger = logger;
        _myDependency = myDependency;
    }

    public IActionResult Index()
    {
        _myDependency.WriteMessage("Hello from HomeController Index action!");
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
