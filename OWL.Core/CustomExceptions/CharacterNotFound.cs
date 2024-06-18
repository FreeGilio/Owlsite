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
        public CharacterNotFoundException() { }

        public CharacterNotFoundException(string name)
        : base(String.Format("Character name cannot be found: {0}", name))
        {

        }

    }
}
