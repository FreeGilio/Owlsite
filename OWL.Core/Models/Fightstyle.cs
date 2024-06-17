using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OWL.Core.Models
{
    public class Fightstyle
    {
        public int Id {  get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 10)]
        public int Power { get; set; }
        [Range(1, 10)]
        public int Speed { get; set; }

        public Fightstyle() { }

        public Fightstyle(FightstyleDto fightstyleDto)
        {
            Id = fightstyleDto.Id;
            Name = fightstyleDto.Name;
            Power = fightstyleDto.Power;
            Speed = fightstyleDto.Speed;
        }

        public static List<Fightstyle> MapToFightstyles(List<FightstyleDto> styleDtos)
        {

            List<Fightstyle> fightstyles = new List<Fightstyle>();

            try
            {
                foreach (FightstyleDto fightstyleDto in styleDtos)
                {
                    fightstyles.Add(new Fightstyle(fightstyleDto));
                }
            }
            catch (Exception ex)
            {

            }


            return fightstyles;
        }
    }
}
