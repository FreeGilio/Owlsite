using OWL.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;

namespace OWL.DataAccess.Repository
{
    public class MoveRepository : IMoveRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public MoveRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
    }
}
