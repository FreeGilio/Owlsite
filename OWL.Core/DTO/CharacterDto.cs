using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.DTO
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }

        public string Image { get; set; }

        public bool NewlyAdded { get; set; }
    }
}
