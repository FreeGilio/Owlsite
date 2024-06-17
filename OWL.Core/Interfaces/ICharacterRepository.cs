﻿using OWL.Core.DTO;
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
        void UpdateNewlyAdded();
        bool AddCharacterDto(CharacterDto characterToAdd);
        void DeleteCharacter(CharacterDto character);
    }
}
