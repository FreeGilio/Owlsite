using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Interfaces
{
    public interface ICharacterRepository
    {
        List<CharacterDto> GetAllCharacters();
    }
}
