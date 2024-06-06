using OWL.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;

namespace OWL.DataAccess.Repository
{
    public class FightstyleRepository : IFightstyleRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public FightstyleRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }


    }
}
