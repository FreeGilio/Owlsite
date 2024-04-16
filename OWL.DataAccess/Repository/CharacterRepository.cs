using OWL.Core.Interfaces;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using OWL.DataAccess.DB;
using OWL.Core.DTO;

namespace OWL.DataAccess.Repository
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public CharacterRepository(DatabaseConnection databaseConnection) 
        {
            this.databaseConnection = databaseConnection;
        }

        public List<CharacterDto> GetAllCharacters()
        {
            List<CharacterDto> characters = new List<CharacterDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT * FROM Character";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        characters.Add(MapCharacterDtoFromReader(reader));
                    }
                }
            });

            return characters;
        }

        private CharacterDto MapCharacterDtoFromReader(SqlDataReader reader)
        {
            return new CharacterDto
            {
                /*Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = (string)reader["Description"],
                Image = (string)reader["Image"],
                NewlyAdded = (bool)reader["NewlyAdded"]*/
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                NewlyAdded = (bool)reader["NewlyAdded"]
            };
        }


    }
}
