using Microsoft.AspNetCore.Mvc;
using OWL.Core.Models;
using OWL.Core.Services;

namespace OWL.MVC.Controllers
{
    public class CharacterController : Controller
    {
        private readonly CharacterService characterService;

        public CharacterController(CharacterService characterService)
        {
            this.characterService = characterService;
        }
        public IActionResult Index()
        {
            var characters = characterService.GetAllCharacters();
            return View(characters);
        }

        public IActionResult CharInfo(int id)
        {
            Character characterModel = characterService.GetCharacterById(id);
            return View(characterModel);
        }
    }
}
