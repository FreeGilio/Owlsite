using OWL.Core.DTO;
using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Interfaces
{
    public interface ICharacterRepository
    {
        CharacterDto GetCharacterDtoById(int charId);
        List<CharacterDto> GetAllCharactersWithFightstyle();

        bool CheckNameExists(CharacterDto character, int? currentCharacterId = null);

        bool CheckFightstyleTiedToCharacter(CharacterDto character, int? currentCharacterId = null);

        void AddCharacterDto(CharacterDto characterToAdd);

        void UpdateCharacterDto(CharacterDto characterToUpdate);
        void DeleteCharacter(CharacterDto character);
    }
}
