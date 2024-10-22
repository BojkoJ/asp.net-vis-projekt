using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using Projekt.Services;

namespace Projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseManager _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DatabaseManager db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var gloves = _db.GetProductsByCategory(1); // Rukavice
            var wraps = _db.GetProductsByCategory(2); // Bandáže
            var protectors = _db.GetProductsByCategory(3); // Chrániče

            // Kombinuj produkty z každé kategorie
            var products = gloves.Concat(wraps).Concat(protectors).ToList();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
