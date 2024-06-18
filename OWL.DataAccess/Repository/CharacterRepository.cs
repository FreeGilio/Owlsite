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
using OWL.Core.CustomExceptions;
using System.Transactions;

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
                            if (string.IsNullOrEmpty(reader.GetString(1)))
                            {
                                throw new CharacterNotFoundException(reader.GetString(1));
                            }
                            else
                            {
                                result = MapCharacterDtoFromReader(reader);
                            }                        
                        }
                    }
                }
            });

            return result;
        }

        public void CheckFightstyleTiedToCharacter(CharacterDto characterToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {
                string checkMatchSql = @"
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
                fs.Name = @FightstyleName;
        ";

                using (SqlCommand checkMatchCommand = new SqlCommand(checkMatchSql, (SqlConnection)connection))
                {
                    checkMatchCommand.Parameters.Add(new SqlParameter("@FightstyleName", characterToAdd.Fightstyle.Name));

                    using (SqlDataReader reader = checkMatchCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            throw new FightstyleTiedToCharacterException("A character is already tied with this fightstyle.", characterToAdd.Name, characterToAdd.Fightstyle.Name);
                        }
                    }
                }
            });
        }

        public void UpdateNewlyAdded()
        {
            databaseConnection.StartConnection(connection =>
            {
                string updatesql = "UPDATE Character SET NewlyAdded = 0 WHERE NewlyAdded = 1;";
                using (SqlCommand command = new SqlCommand(updatesql, (SqlConnection)connection))
                {
                    command.ExecuteNonQuery();
                }
            });
          
        }

        public void AddCharacterDto(CharacterDto characterToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checknameSql = "SELECT COUNT(*) FROM Character WHERE Name = @Name;";
                using (SqlCommand checkCommand = new SqlCommand(checknameSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Name", characterToAdd.Name));
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("A character with this name already exists.", characterToAdd.Name);
                    }
                }

                CheckFightstyleTiedToCharacter(characterToAdd);
                UpdateNewlyAdded();         

                string insertSql = "INSERT INTO Character (Name, Image, Description, NewlyAdded, Fightstyle_id) VALUES (@Name, @Image, @Description, @NewlyAdded, @Fightstyle_id);";
                        using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                        {
                            insertCommand.Parameters.Add(new SqlParameter("@Name", characterToAdd.Name));
                            insertCommand.Parameters.Add(new SqlParameter("@Image", characterToAdd.Image));
                            insertCommand.Parameters.Add(new SqlParameter("@Description", characterToAdd.Description));
                            insertCommand.Parameters.Add(new SqlParameter("@NewlyAdded", true));
                            insertCommand.Parameters.Add(new SqlParameter("@Fightstyle_id", characterToAdd.Fightstyle.Id));

                            insertCommand.ExecuteNonQuery();
                            
                }                             
            });
        }

        public void DeleteCharacter(CharacterDto charDto)
        {
            databaseConnection.StartConnection(connection =>
            {

                // Delete the character
                string deleteSql = "DELETE FROM Character WHERE Id = @Id;";
                using (SqlCommand deleteCommand = new SqlCommand(deleteSql, (SqlConnection)connection))
                {
                    deleteCommand.Parameters.Add(new SqlParameter("@Id", charDto.Id));
                    deleteCommand.ExecuteNonQuery();
                }

                // Optionally reset the auto-increment value if required
                // This step is usually not necessary as SQL Server handles it automatically
                string resetAutoIncrementSql = @"
                    DECLARE @MaxId INT;
                    SELECT @MaxId = ISNULL(MAX(Id), 0) FROM Character;
                    DBCC CHECKIDENT ('Character', RESEED, @MaxId);
                ";
                using (SqlCommand resetCommand = new SqlCommand(resetAutoIncrementSql, (SqlConnection)connection))
                {
                    resetCommand.ExecuteNonQuery();
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
                string sql = "SELECT Name, Image, Description, NewlyAdded FROM Character";
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
