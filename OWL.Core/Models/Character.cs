using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Character
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public string Image { get; set; }

        public bool NewlyAdded { get; set; }

        public Fightstyle FightStyle { get; set; }


        public Character(int id, string name, string description, string image, bool newlyAdded, Fightstyle fightstyle)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
            FightStyle = fightstyle;
        }
        public Character(CharacterDto characterDto)
        {
            Id = characterDto.Id;
            Name = characterDto.Name;
            Description = characterDto.Description;
            Image = characterDto.Image;
            NewlyAdded = characterDto.NewlyAdded;
            FightStyle = characterDto.Fightstyle;
        }

        public Character()
        {

        }


        public static List<Character> MapToCharacters(List<CharacterDto> characterDtos)
        {

            List<Character> characters = new List<Character>();

            try
            {
                foreach (CharacterDto characterDto in characterDtos)
                {
                    characters.Add(new Character(characterDto));
                }
            }
            catch (Exception ex) 
            { 

            }
      

            return characters;
        }
       
    }
}
