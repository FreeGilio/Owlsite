using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.DataAccess.DB;

namespace OWL.DataAccess.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public NewsRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }
    }
}
