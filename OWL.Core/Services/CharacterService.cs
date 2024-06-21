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
using Microsoft.AspNetCore.Http;


namespace OWL.Core.Services
{
    public class CharacterService
    {
        private readonly ICharacterRepository _characterRepo;

        public CharacterService(ICharacterRepository characterRepository)
        {
            this._characterRepo = characterRepository;
        }

        public Character GetCharacterById(int? charId)
        {
            if (!charId.HasValue)
            {
                throw new IdNotFoundException("Character ID has not been provided.");
            }

            CharacterDto charDto = _characterRepo.GetCharacterDtoById(charId.Value);
            if (charDto == null)
            {
                throw new IdNotFoundException(charId.Value);
            }

            return new Character(charDto);
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

        public void AddCharacterImage(Character character, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Validate file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    throw new InvalidImageExtensionException("Invalid image file type.");
                }

                // Validate MIME type
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                if (!allowedMimeTypes.Contains(imageFile.ContentType))
                {
                    throw new InvalidImageTypeException("Invalid image MIME type.");
                }

                // Generate a unique file name
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";

                // Set the path to store the file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Characters", uniqueFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                // Save the path in the model
                character.Image = $"/Images/Characters/{uniqueFileName}";
            }
            else
            {
                character.Image = $"/Images/Characters/Unknown.png";
            }
        }

        public void RemoveCharacterImage(Character character)
        {
            // Delete associated image file if it exists
            if (!string.IsNullOrEmpty(character.Image))
            {
                if (character.Image != "/Images/Characters/Unknown.png")
                {
                    try
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", character.Image.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch (ImageNotDeletedException ex)
                    {
                        Console.WriteLine($"Error deleting image file: {ex.Message}");
                    }
                }

            }
        }

        public void AddCharacter(Character characterToAdd)
        {

            if (string.IsNullOrEmpty(characterToAdd.Name))
            {
                throw new NameRequiredException("Character name is required.");
            }

            if (_characterRepo.CheckNameExists(new CharacterDto
            {
                Name = characterToAdd.Name,
                Id = characterToAdd.Id
            }))
            {
                throw new NameExistsException("A character with this name already exists.", characterToAdd.Name);
            }

            if (_characterRepo.CheckFightstyleTiedToCharacter(new CharacterDto
            {
                Name = characterToAdd.Name,
                Id = characterToAdd.Id,
                Fightstyle = characterToAdd.FightStyle
            }))
            {
                throw new FightstyleTiedToCharacterException(characterToAdd.Name,characterToAdd.FightStyle.Name);
            }

            CharacterDto charDto = new CharacterDto(characterToAdd);   
            _characterRepo.AddCharacterDto(charDto);            
        }


        public void UpdateCharacter(Character characterToUpdate)
        {

            if (string.IsNullOrEmpty(characterToUpdate.Name))
            {
                throw new NameRequiredException("Character name is required.");
            }

            if (_characterRepo.CheckNameExists(new CharacterDto
            {
                Name = characterToUpdate.Name,
                Id = characterToUpdate.Id
            }, characterToUpdate.Id))
            {
                throw new NameExistsException("A character with this name already exists.", characterToUpdate.Name);
            }

            if (_characterRepo.CheckFightstyleTiedToCharacter(new CharacterDto
            {
                Name = characterToUpdate.Name,
                Id = characterToUpdate.Id,
                Fightstyle = characterToUpdate.FightStyle
            }, characterToUpdate.Id))
            {
                throw new FightstyleTiedToCharacterException(characterToUpdate.Name, characterToUpdate.FightStyle.Name);
            }

            CharacterDto charDto = new CharacterDto(characterToUpdate);
             _characterRepo.UpdateCharacterDto(charDto);
            
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

        public List<Move> GetMovesForCharacter(int characterId)
        {
            try
            {
                List<MoveDto> moveDtos = _characterRepo.GetMovesForCharacter(characterId);
                return Move.MapToMoves(moveDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error getting moves for character with ID {characterId}", ex);
                throw;
            }
        }

    }
}
 