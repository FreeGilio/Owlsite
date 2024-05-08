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
        private readonly ICharacterRepository _characterRepo;

        public CharacterService(ICharacterRepository characterRepository)
        {
            this._characterRepo = characterRepository;
        }

        public Character GetCharacterById(int charId)
        {
            CharacterDto charDto = _characterRepo.GetCharacterDtoById(charId);
           
            return new Character(charDto);
        }

        public List<Character> GetAllCharactersWithFightstyle()
        {           
                List<CharacterDto> characterDtos = _characterRepo.GetAllCharactersWithFightstyle();
                return Character.MapToCharacters(characterDtos);                   
        }
    }
}
 