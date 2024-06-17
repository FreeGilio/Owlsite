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
                    throw new CharacterNotFoundException("Character ID has not been found", charId.ToString());
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

        public void UpdateNewlyAdded()
        {
            try
            {
               _characterRepo.UpdateNewlyAdded();
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error updating the 'NewlyAdded' column", ex);
                throw; 
            }
        }


        public void AddCharacter(Character characterToAdd)
        {

            if (string.IsNullOrEmpty(characterToAdd.Name))
            {
                throw new ArgumentException("Character name is required.");
            }
            else
            {
                CharacterDto charDto = new CharacterDto
                {
                    Name = characterToAdd.Name,
                    Description = characterToAdd.Description,
                    Image = characterToAdd.Image,
                    NewlyAdded = characterToAdd.NewlyAdded,
                    Fightstyle = new Fightstyle
                    {
                        Id = characterToAdd.FightStyle.Id,
                        Name = characterToAdd.FightStyle.Name,
                        Power = characterToAdd.FightStyle.Power,
                        Speed = characterToAdd.FightStyle.Speed
                    }
                };
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
 