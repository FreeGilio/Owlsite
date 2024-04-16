using OWL.Core.DTO;
using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Mappers
{
    public class CharacterMapper
    {
        /* public Character MapToCharacter(CharacterDto characterDto)
         {
             if (characterDto == null)
             {
                 return null;
             }

             return new Character
             {
                 Id = characterDto.Id,
                 Name = characterDto.Name,
                 Description = characterDto.Description,
                 Image = characterDto.Image,
                 NewlyAdded = characterDto.NewlyAdded,
             };
         }

         public List<Character> MapToCharacters(List<CharacterDto> characterDtoList)
         {
             if (characterDtoList == null)
             {
                 return new List<Character>();
             }

             List<Character> characters = new List<Character>();

             foreach (CharacterDto characterDto in characterDtoList)
             {
                 Character convertedChar = MapToCharacter(characterDto);
                 if (convertedChar != null)
                 {
                     characters.Add(convertedChar);
                 }
             }

             return characters;
         }

         public CharacterDto MapToCharacterDto(Character character)
         {
             if (character == null)
             {
                 return null;
             }

             return new CharacterDto
             {
                 Id = character.Id,
                 Name = character.Name,
                 Description = character.Description,
                 Image = character.Image,
                 NewlyAdded = character.NewlyAdded,
             };
         }

         public List<CharacterDto> MapToCharacterDtos(List<Character> characterList)
         {
             if (characterList == null)
             {
                 return new List<CharacterDto>();
             }

             List<CharacterDto> characterDtos = new List<CharacterDto>();

             if (characterList != null)
             {
                 foreach (Character characterObj in characterList)
                 {
                     CharacterDto convertedCharDto = MapToCharacterDto(characterObj);
                     if (convertedCharDto != null)
                     {
                         characterDtos.Add(convertedCharDto);
                     }
                 }
             }

             return characterDtos;
         }*/
    }
}
