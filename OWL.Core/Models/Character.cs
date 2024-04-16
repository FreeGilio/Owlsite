using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Character
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public string Image { get; private set; }

        public bool NewlyAdded { get; private set; }

        public Character(int id, string name, string description, string image, bool newlyAdded)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            NewlyAdded = newlyAdded;
        }
        public Character(CharacterDto characterDto)
        {
            Id = characterDto.Id;
            Name = characterDto.Name;
            Description = characterDto.Description;
            Image = characterDto.Image;
            NewlyAdded = characterDto.NewlyAdded;    
        }

        public static List<Character> MapToCharacters(List<CharacterDto> characterDtos)
        {
            List<Character> characters = new List<Character>();

            foreach (CharacterDto characterDto in characterDtos)
            {
                characters.Add(new Character(characterDto));
            }

            return characters;
        }
       
    }
}
