using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OWL.Core.CustomExceptions;
using OWL.Core.Models;
using OWL.Core.Services;
using System.Security.Claims;

namespace OWL.MVC.Controllers
{
    public class CharacterController : Controller
    {
        private readonly CharacterService characterService;
        private readonly FightstyleService fightstyleService;

        public CharacterController(CharacterService characterService, FightstyleService fightstyleService)
        {
            this.characterService = characterService;
            this.fightstyleService = fightstyleService;
        }

        public ActionResult CharInfo(int id)
        {
            Character characterModel = characterService.GetCharacterById(id);
            return View(characterModel);
        }

        [HttpGet]
        public IActionResult AddChar()
        {
            var fightstyles = fightstyleService.GetAllFightstyles();
            ViewBag.Fightstyles = new SelectList(fightstyles, "Id", "Name");
            return View(new Character());
        }

        [HttpPost]
        public IActionResult AddChar(Character characterToBeAdded, int selectedStyleId)
        {   
                try
                {
                  try
                  {
                    characterToBeAdded.FightStyle = fightstyleService.GetFightstyleById(selectedStyleId);
                    characterService.AddCharacter(characterToBeAdded);
                    return RedirectToAction("Index", "Character");
                  }
                  catch (FightstyleTiedToCharacterException ex)
                  {
                    ModelState.AddModelError("Name", ex.Message);
                  }
                   
                }
                catch (NameExistsException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);
                }           

            var fightstyles = fightstyleService.GetAllFightstyles();
            ViewBag.Fightstyles = new SelectList(fightstyles, "Id", "Name");
            return View(characterToBeAdded);
        }

        public IActionResult Index()
        {
            var characters = characterService.GetAllCharactersWithFightstyle();
            return View(characters);
        }

        [HttpPost]
        public IActionResult DeleteCharacter(int id)
        {
            var character = characterService.GetCharacterById(id);
            if (character != null)
            {
                characterService.DeleteCharacter(character);
                return RedirectToAction("Index", "Character");
            }
            return NotFound();
        }

    }
}
