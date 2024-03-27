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
        [Key]
        public int Character_Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public string Image {  get; private set; }

        public bool NewlyAdded {  get; private set; }

        public List<Move> Moves { get; private set; }
       
    }
}
