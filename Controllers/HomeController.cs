using iiits_face_recognition_core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;

        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // New Registration Page
        public IActionResult New()
        {
            return View();
        }

        // Registered List Page
        public async Task<IActionResult> List()
        {
            var persons = await _context.Persons.ToListAsync();
            return View(persons);
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
