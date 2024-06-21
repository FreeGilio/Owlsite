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

        public FightstyleTiedToCharacterException(string characterName, string fightstyleName) : base(String.Format("A character is already tied with this fightstyle.", characterName, fightstyleName))
        {
            CharacterName = characterName;
            FightstyleName = fightstyleName;
        }
    }
}
