using OWL.Core.Interfaces;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using OWL.DataAccess.DB;

namespace OWL.DataAccess.Repository
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public CharacterRepository(DatabaseConnection databaseConnection) 
        {
            this.databaseConnection = databaseConnection;
        }


    }
}
