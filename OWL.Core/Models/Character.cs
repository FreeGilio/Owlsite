using System;
using System.Collections.Generic;
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

        public string Image {  get; private set; }

        public bool NewlyAdded {  get; private set; }

        public Move Moves { get; private set; }

        public Fightstyle Fightstyle { get; private set; }
    }
}
