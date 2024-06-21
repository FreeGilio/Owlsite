using OWL.Core.Interfaces;
using OWL.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.DataAccess.Repository
{
    public class GamemodeRepository : IGamemodeRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public GamemodeRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
    }
}
