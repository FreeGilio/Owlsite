using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.Models;
using OWL.Core.DTO;
using OWL.Core.Logger;
using OWL.Core.CustomExceptions;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;


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
                if (string.IsNullOrEmpty(charId.ToString()))
                {
                    throw new IdNotFoundException("Character ID has not been found", charId);
                }
                else
                {
                    CharacterDto charDto = _characterRepo.GetCharacterDtoById(charId);
                    return new Character(charDto);
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
                throw;
            }                 
        }

        public void AddCharacter(Character characterToAdd)
        {

            if (string.IsNullOrEmpty(characterToAdd.Name))
            {
                throw new NameRequiredException("Character name is required.");
            }
            else
            {
                CharacterDto charDto = new CharacterDto(characterToAdd);   
                _characterRepo.AddCharacterDto(charDto);
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
                OwlLogger.LogError($"Error deleting character {characterToRemove.Name}", ex);
                throw; // Optionally rethrow the exception
            }
        }

    }
}
 