using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OWL.Core.DTO
{
    public class FightstyleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [Range(1, 10)]
        public int Power { get; set; }
        [Range(1, 10)]
        public int Speed { get; set; }

        public FightstyleDto() { }

        public FightstyleDto(Fightstyle fightstyle)
        {
            Id = fightstyle.Id;
            Name = fightstyle.Name;
            Power = fightstyle.Power;
            Speed = fightstyle.Speed;
        }
    }
}
