using OWL.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Services
{
    public class GamemodeService
    {

        private readonly IGamemodeRepository _gamemodeRepo;

        public GamemodeService(IGamemodeRepository gamemodeRepository)
        {
            this._gamemodeRepo = gamemodeRepository;
        }


    }
}
