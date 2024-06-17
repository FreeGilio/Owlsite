using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    [Serializable]
    public class CharacterNotFoundException : Exception
    {
        public string Name { get; }

        public CharacterNotFoundException() { }

        public CharacterNotFoundException(string message)
            : base(message) { }

        public CharacterNotFoundException(string message, string name)
            : this(message)
        {
            Name = name;
        }
    }
}
