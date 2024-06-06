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
using OWL.Core.Models;

namespace OWL.DataAccess.Repository
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public CharacterRepository(DatabaseConnection databaseConnection) 
        {
            this.databaseConnection = databaseConnection;
        }

        public CharacterDto GetCharacterDtoById(int charId)
        {
            CharacterDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT
                    c.Id as Id,
                    c.Name as Name,
                    c.Description,
                    c.Image,
                    c.NewlyAdded,
                    fs.Id as FightstyleId,
                    fs.Name as FightstyleName,
                    fs.Power,
                    fs.Speed
                FROM 
                    Character c
                JOIN
                    Fightstyle fs ON c.fightstyle_id = fs.Id
                WHERE 
                    c.Id = @Id";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", charId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = MapCharacterDtoFromReader(reader);
                        }
                    }
                }
            });

            return result;
        }

        public int AddCharacterDto(CharacterDto characterToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {   
                       
                        string sql = "INSERT INTO character (Name, Image, Description, NewlyAdded) VALUES (@Name, @Image, @Description, @NewlyAdded); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                        {
                            command.Parameters.Add(new SqlParameter("@Name", characterToAdd.Name));
                            command.Parameters.Add(new SqlParameter("@Image", characterToAdd.Image));
                            command.Parameters.Add(new SqlParameter("@Description", characterToAdd.Description));
                            command.Parameters.Add(new SqlParameter("@NewlyAdded", characterToAdd.NewlyAdded));

                            int characterId = Convert.ToInt32(command.ExecuteScalar());

                            characterToAdd.Id = characterId;
                        }                             
            });
            return characterToAdd.Id;
        }

        public void DeleteCharacter(CharacterDto charDto)
        {
            databaseConnection.StartConnection(connection =>
            {
                string sql = "DELETE FROM character WHERE id = @id;";

                using (SqlCommand command = new(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@id", charDto.Id));

                    command.ExecuteNonQuery();
                }
            });
        }

        public List<CharacterDto> GetAllCharactersWithFightstyle()
        {
            List<CharacterDto> characters = new List<CharacterDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT 
                    c.Id as Id,
                    c.Name as Name,
                    c.Description,
                    c.Image,
                    c.NewlyAdded,
                    fs.Id as FightstyleId,
                    fs.Name as FightstyleName,
                    fs.Power,
                    fs.Speed
                FROM 
                    Character c
                JOIN
                    Fightstyle fs ON c.fightstyle_id = fs.Id";
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

        public List<CharacterDto> GetAllCharacters()
        {
            List<CharacterDto> characters = new List<CharacterDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Name, Image, Description, NewlyAdded FROM character";
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
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                NewlyAdded = (bool)reader["NewlyAdded"],
                Fightstyle = new Fightstyle
                {
                    Id = (int)reader["FightstyleId"],
                    Name = (string)reader["FightstyleName"],
                    Power = (int)reader["Power"],
                    Speed = (int)reader["Speed"]
                }
            };
        }


    }
}
