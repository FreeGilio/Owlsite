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

        public bool CheckNameExists(CharacterDto character, int? currentCharacterId = null)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checknameSql = "SELECT COUNT(*) FROM Character WHERE Name = @Name";

                if (currentCharacterId.HasValue)
                {
                    checknameSql += " AND Id != @CurrentCharacterId";
                }

                using (SqlCommand checkCommand = new SqlCommand(checknameSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Name", character.Name));

                    if (currentCharacterId.HasValue)
                    {
                        checkCommand.Parameters.Add(new SqlParameter("@CurrentCharacterId", currentCharacterId.Value));
                    }

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("A character with this name already exists.", character.Name);
                    }
                }             
            });
            return false;
        }

        public bool CheckFightstyleTiedToCharacter(CharacterDto characterToAdd, int? currentCharacterId = null)
        {
            bool isTied = false;

            databaseConnection.StartConnection(connection =>
            {
                string checkMatchSql = @"
            SELECT c.Id as Id, 
                c.Name as Name, 
                c.Description, 
                c.Image, 
                c.NewlyAdded,
                   fs.Id as FightstyleId, 
                   fs.Name as FightstyleName, 
                   fs.Power, 
                   fs.Speed
            FROM Character c
            LEFT JOIN Fightstyle fs ON c.fightstyle_id = fs.Id
            WHERE fs.Name = @FightstyleName
            ";

                if (currentCharacterId.HasValue)
                {
                    checkMatchSql += " AND c.Id != @CurrentCharacterId";
                }

                using (SqlCommand checkMatchCommand = new SqlCommand(checkMatchSql, (SqlConnection)connection))
                {
                    checkMatchCommand.Parameters.Add(new SqlParameter("@FightstyleName", characterToAdd.Fightstyle.Name));

                    if (currentCharacterId.HasValue)
                    {
                        checkMatchCommand.Parameters.Add(new SqlParameter("@CurrentCharacterId", currentCharacterId.Value));
                    }

                    using (SqlDataReader reader = checkMatchCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isTied = true; // Set to true if a match is found
                                           //throw new FightstyleTiedToCharacterException("A character is already tied with this fightstyle.", characterToAdd.Name, characterToAdd.Fightstyle.Name);

                        }
                    }
                }
            });
            return isTied;
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
              
                CheckNameExists(characterToAdd);
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

        public void UpdateCharacterDto(CharacterDto characterToUpdate)
        {
            databaseConnection.StartConnection(connection =>
            {
                CheckNameExists(characterToUpdate, characterToUpdate.Id);
                CheckFightstyleTiedToCharacter(characterToUpdate, characterToUpdate.Id);

                string updateSql = "UPDATE Character SET Name = @Name, Image = @Image, Description = @Description, NewlyAdded = @NewlyAdded, Fightstyle_id = @Fightstyle_id WHERE Id = @Id;";
                using (SqlCommand updateCommand = new SqlCommand(updateSql, (SqlConnection)connection))
                {
                    updateCommand.Parameters.Add(new SqlParameter("@Id", characterToUpdate.Id));
                    updateCommand.Parameters.Add(new SqlParameter("@Name", characterToUpdate.Name));
                    updateCommand.Parameters.Add(new SqlParameter("@Image", characterToUpdate.Image));
                    updateCommand.Parameters.Add(new SqlParameter("@Description", characterToUpdate.Description));
                    updateCommand.Parameters.Add(new SqlParameter("@NewlyAdded", false));
                    updateCommand.Parameters.Add(new SqlParameter("@Fightstyle_id", characterToUpdate.Fightstyle.Id));

                    updateCommand.ExecuteNonQuery();

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

        public List<MoveDto> GetMovesForCharacter(int characterId)
        {
            List<MoveDto> moves = new List<MoveDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
            SELECT m.Id, m.Name, m.Description, m.Image, m.Motion
            FROM Move m
            JOIN CharacterMove cm ON m.Id = cm.Move_Id
            WHERE cm.Character_Id = @CharacterId";

                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@CharacterId", characterId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            moves.Add(new MoveDto
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                                Motion = reader.IsDBNull(reader.GetOrdinal("Motion")) ? null : (string)reader["Motion"]
                            });
                        }
                    }
                }
            });

            return moves;
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
