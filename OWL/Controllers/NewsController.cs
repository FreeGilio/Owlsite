using Microsoft.AspNetCore.Mvc;
using OWL.Core.Models;
using OWL.Core.Services;

namespace OWL.MVC.Controllers
{

    public class NewsController : Controller
    {

        private readonly NewsService newsService;
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult NewsInfo(int id)
        {
            //News newsModel = newsService.GetNewsById(id);
            return View();
        }
    }
}
