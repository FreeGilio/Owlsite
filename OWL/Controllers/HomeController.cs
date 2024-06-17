using Microsoft.AspNetCore.Mvc;
using OWL.Models;
using OWL.Core.Models;
using OWL.Core.Services;
using System.Diagnostics;

namespace OWL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly NewsService newsService;

        public HomeController(ILogger<HomeController> logger, NewsService newsService)
        {
            _logger = logger;
            this.newsService = newsService;
        }

        public IActionResult Index()
        {
            var news = newsService.GetAllNewsWithCategories();
            return View(news);
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
}