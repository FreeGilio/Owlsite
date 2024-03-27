using Microsoft.AspNetCore.Mvc;
using OWL.Core.Models;

namespace OWL.MVC.Controllers
{
    public class CharacterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
