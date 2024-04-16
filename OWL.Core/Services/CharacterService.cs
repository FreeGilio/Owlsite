using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.DTO;


namespace OWL.Core.Services
{
    public class CharacterService
    {
        private readonly ICharacterRepository characterRepository;

        public CharacterService(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public List<Character> GetAllCharacters()
        {           
                List<CharacterDto> characterDtos = characterRepository.GetAllCharacters();
                return Character.MapToCharacters(characterDtos);                   
        }
    }
}
 