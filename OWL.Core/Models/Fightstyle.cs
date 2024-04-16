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
        public int Id {  get; private set; }

        public string Name { get; private set; }

        public int Power { get; private set; }

        public int Speed { get; private set; }
    }
}
