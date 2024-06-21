using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using OWL.Core.CustomExceptions;
using OWL.Core.Models;
using OWL.Core.Services;
using System.Linq;
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
            var fightstyles = fightstyleService.GetAllFightstylesNotMatchingCharacter();
            ViewBag.Fightstyles = new SelectList(fightstyles, "Id", "Name");
            return View(new Character());
        }

        [HttpPost]
        public IActionResult AddChar(Character characterToBeAdded, int selectedStyleId, IFormFile imageFile)
        {
            try
            {             
                    characterService.AddCharacterImage(characterToBeAdded, imageFile);
                    characterToBeAdded.FightStyle = fightstyleService.GetFightstyleById(selectedStyleId);
                    characterService.AddCharacter(characterToBeAdded);
                    return RedirectToAction("Index", "Character");
            }

            catch (FightstyleTiedToCharacterException ex)
            {
                ModelState.AddModelError("selectedStyleId", ex.Message);
            }

            catch (NameExistsException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
            }           

            var fightstyles = fightstyleService.GetAllFightstylesNotMatchingCharacter();
            ViewBag.Fightstyles = new SelectList(fightstyles, "Id", "Name");
            return View(characterToBeAdded);
        }

        [HttpGet]
        public IActionResult EditChar(int id)
        {
           
            Character characterModel = characterService.GetCharacterById(id);
            ViewBag.Fightstyles = new SelectList(fightstyleService.GetAllFightstyles(), "Id", "Name", characterModel.FightStyle.Id);
            return View(characterModel);
        }

        [HttpPost]
        public IActionResult EditChar(Character characterToBeUpdated, int selectedStyleId, IFormFile imageFile)
        {
            try
            {
                // Retrieve current character details to get the old image path
                var currentCharacter = characterService.GetCharacterById(characterToBeUpdated.Id);

                if (imageFile != null && imageFile.Length > 0)
                {
                    characterService.RemoveCharacterImage(currentCharacter);
                    characterService.AddCharacterImage(characterToBeUpdated, imageFile);
                }
                else
                {
                    // Keep the current image if no new image is uploaded
                    characterToBeUpdated.Image = currentCharacter.Image;
                }

                characterToBeUpdated.FightStyle = fightstyleService.GetFightstyleById(selectedStyleId);
                characterService.UpdateCharacter(characterToBeUpdated);
                return RedirectToAction("Index", "Character");
            }
            catch (FightstyleTiedToCharacterException ex)
            {
                ModelState.AddModelError("selectedStyleId", ex.Message); 
            }
            catch (NameExistsException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
            }

            var fightstyles = fightstyleService.GetAllFightstyles();
            ViewBag.Fightstyles = new SelectList(fightstyles, "Id", "Name");
            return View(characterToBeUpdated);
        }

        public IActionResult Index()
        {
            var characters = characterService.GetAllCharactersWithFightstyle();
            return View(characters);
        }

        [HttpPost]
        public IActionResult DeleteCharacter(int id)
        {
            // Retrieve character details
            var character = characterService.GetCharacterById(id);
            if (character == null)
            {
                return NotFound();
            }

            characterService.RemoveCharacterImage(character);

            // Delete character from the database
            characterService.DeleteCharacter(character);

            return RedirectToAction("Index", "Character");
        }

        public IActionResult CharacterMoves(int id)
        {
            try
            {
                Character characterModel = characterService.GetCharacterById(id);
                List<Move> moves = characterService.GetMovesForCharacter(id);

                Tuple<string, List<Move>> model = new Tuple<string, List<Move>>(characterModel.Name, moves);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Character"); // Redirect to a safe page
            }
        }

    }
}
