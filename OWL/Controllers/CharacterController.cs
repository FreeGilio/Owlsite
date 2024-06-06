using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OWL.Core.Models;
using OWL.Core.Services;
using System.Security.Claims;

namespace OWL.MVC.Controllers
{
    public class CharacterController : Controller
    {
        private readonly CharacterService characterService;

        public CharacterController(CharacterService characterService)
        {
            this.characterService = characterService;
        }
        public ActionResult Index()
        {
            var characters = characterService.GetAllCharactersWithFightstyle();
            return View(characters);
        }

        public ActionResult CharInfo(int id)
        {
            Character characterModel = characterService.GetCharacterById(id);
            return View(characterModel);
        }

        public void btnAddChar_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddChar.cshtml");
        }
            
        public void AddChar(string name, string image, string description, bool newlyAdded)
        {

            Character characterModel = new Character
            {
                Name = name,
                Image = image,
                Description = description,
                NewlyAdded = newlyAdded
            };
            characterService.AddCharacter(characterModel);
        }
    }
}
