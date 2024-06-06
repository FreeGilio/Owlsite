using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.DTO;
using OWL.Core.Logger;
using System.Reflection.Metadata.Ecma335;


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
            try
            {
                CharacterDto charDto = _characterRepo.GetCharacterDtoById(charId);
                return new Character(charDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error getting character by ID {charId}", ex);
                throw; 
            }
        }

        public List<Character> GetAllCharactersWithFightstyle()
        {
            try
            {
                List<CharacterDto> characterDtos = _characterRepo.GetAllCharactersWithFightstyle();
                return Character.MapToCharacters(characterDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error getting all characters with fight style", ex);
                throw; // Optionally rethrow the exception or return an empty list
            }                 
        }

        public void AddCharacter(Character characterToAdd)
        {
            try
            {
                if (string.IsNullOrEmpty(characterToAdd.Name))
                {
                    throw new ArgumentException("Character name cannot be null or empty");
                }

                CharacterDto charDto = new CharacterDto() { };
                _characterRepo.AddCharacterDto(charDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error adding character {characterToAdd}", ex);
                throw; 
            }
      
        }

        public void DeleteCharacter(Character characterToRemove)
        {                      
            try
            {
                CharacterDto characterDto = new CharacterDto(characterToRemove) { };
                _characterRepo.DeleteCharacter(characterDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error deleting character {characterToRemove}", ex);
                throw; // Optionally rethrow the exception
            }
        }

    }
}
 