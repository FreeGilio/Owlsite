using Microsoft.AspNetCore.Mvc;
using OWL.Core.Models;
using OWL.Core.Services;

namespace OWL.MVC.Controllers
{
    public class FightstyleController : Controller
    {

        private readonly FightstyleService fightstyleService;

        public FightstyleController(FightstyleService fightstyleService)
        {
            this.fightstyleService = fightstyleService;
        }

        [HttpGet]
        public IActionResult AddFightstyle()
        {
            return View(new Fightstyle());
        }

        [HttpPost]
        public IActionResult CreateFightstyle(Fightstyle styleToBeAdded)
        {
            if (ModelState.IsValid)
            {
                fightstyleService.AddFightstyle(styleToBeAdded);
                return RedirectToAction("Index", "Character");
            }

            return View(styleToBeAdded);
        }
    }
}
