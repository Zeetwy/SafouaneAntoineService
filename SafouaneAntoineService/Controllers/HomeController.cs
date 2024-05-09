using Microsoft.AspNetCore.Mvc;
using SafouaneAntoineService.Models;
using System.Diagnostics;

namespace SafouaneAntoineService.Controllers
{
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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Home()
        {
            string? user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Message"] = "You need to be logged in to view this page.";
                return RedirectToAction("Authenticate", "User");
            }
            return View("Views/Home/Home.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
