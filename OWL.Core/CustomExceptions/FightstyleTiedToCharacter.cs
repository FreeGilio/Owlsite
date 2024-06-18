using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.CustomExceptions
{
    public class FightstyleTiedToCharacterException : Exception
    {
        public string CharacterName { get; }
        public string FightstyleName { get; }

        public FightstyleTiedToCharacterException(string message, string characterName, string fightstyleName) : base(message)
        {
            CharacterName = characterName;
            FightstyleName = fightstyleName;
        }
    }
}
