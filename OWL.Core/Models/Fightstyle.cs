using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Fightstyle
    {
        public int Id {  get; set; }

        public string Name { get; set; }

        public int Power { get; set; }

        public int Speed { get; set; }
    }
}
