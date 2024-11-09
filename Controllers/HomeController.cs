using iiits_face_recognition_core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace iiits_face_recognition_core.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class RegistrationController : Controller
    {
        // New Registration Page
        public IActionResult New()
        {
            return View();
        }

        // Registered List Page
        public IActionResult List()
        {
            return View();
        }
    }

    public class RecognizeController : Controller
    {
        public IActionResult Recognize()
        {
            return View();
        }
    }


}
