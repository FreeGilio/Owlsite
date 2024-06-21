using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Interfaces
{
    public interface IMoveRepository
    {
        List<MoveDto> GetAllUniversalMoves();

        void AddMoveDto(MoveDto moveToAdd, int charId);

        bool CheckNameExists(MoveDto move, int? currentMoveId = null);
    }
}
