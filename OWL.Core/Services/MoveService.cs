using OWL.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Services
{
    public class MoveService
    {
        private readonly IMoveRepository _moveRepo;

        public MoveService(IMoveRepository moveRepository)
        {
            this._moveRepo = moveRepository;
        }
    }
}
