using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OWL.Core.CustomExceptions;
using OWL.Core.Models;
using OWL.Core.Services;

namespace OWL.MVC.Controllers
{
    public class MoveController : Controller
    {    
        private readonly MoveService moveService;
        private readonly CharacterService characterService;
        public MoveController(MoveService moveService, CharacterService characterService)
        {
            this.moveService = moveService;
            this.characterService = characterService;
        }


        public IActionResult Index()
        {
            var moves = moveService.GetAllUniversalMoves();
            return View(moves);
        }

        [HttpGet]
        public IActionResult AddMove()
        {
            var characters = characterService.GetAllCharactersWithFightstyle();
            ViewBag.characters = new SelectList(characters, "Id", "Name");
            return View(new Move());
        }

        [HttpPost]
        public IActionResult AddMove(Move moveToBeAdded, int selectedCharId, IFormFile imageFile)
        {
            try
            {
                moveService.AddMoveImage(moveToBeAdded, imageFile);
                moveService.AddMove(moveToBeAdded,selectedCharId);
                return RedirectToAction("Index", "Move");
            }
          
            catch (NameExistsException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
            }

            var characters = characterService.GetAllCharactersWithFightstyle();
            ViewBag.characters = new SelectList(characters, "Id", "Name");
            return View(moveToBeAdded);
        }

    }
}
