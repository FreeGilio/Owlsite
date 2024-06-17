using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Models;

namespace OWL.Core.DTO
{
    public class CharacterDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get;  set; }
        public string Description { get; set; }

        public string Image { get; set; }

        public bool NewlyAdded { get; set; }
        public Fightstyle Fightstyle { get; set; }

        public CharacterDto() { }

        public CharacterDto(Character character)
        {
            Id = character.Id;
            Name = character.Name;
            Description = character.Description;
            Image = character.Image;
            NewlyAdded = character.NewlyAdded;
            Fightstyle = character.FightStyle;
        }

    }
}
