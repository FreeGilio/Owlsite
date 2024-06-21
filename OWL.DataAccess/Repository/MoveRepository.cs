using OWL.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.DTO;
using System.Data.SqlClient;
using OWL.Core.Models;
using System.Transactions;
using OWL.Core.CustomExceptions;

namespace OWL.DataAccess.Repository
{
    public class MoveRepository : IMoveRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public MoveRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public List<MoveDto> GetAllUniversalMoves()
        {
            List<MoveDto> moves= new List<MoveDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT m.Id, m.Name, m.Description, m.Image, m.Motion
                FROM Move m
                LEFT JOIN CharacterMove cm ON m.Id = cm.Move_Id
                WHERE cm.Character_Id IS NULL";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            moves.Add(MapMoveDtoFromReader(reader));
                        }
                    }
                }
               
            });

            return moves;
        }

        public bool CheckNameExists(MoveDto move, int? currentMoveId = null)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checknameSql = "SELECT COUNT(*) FROM Move WHERE Name = @Name";

                if (currentMoveId.HasValue)
                {
                    checknameSql += " AND Id != @CurrentMoveId";
                }

                using (SqlCommand checkCommand = new SqlCommand(checknameSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Name", move.Name));

                    if (currentMoveId.HasValue)
                    {
                        checkCommand.Parameters.Add(new SqlParameter("@CurrentCharacterId", currentMoveId.Value));
                    }

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("A move with this name already exists.", move.Name);
                    }
                }
            });
            return false;
        }

        public void AddMoveDto(MoveDto moveToAdd,int charId)
        {
            databaseConnection.StartConnection(connection =>
            {
                CheckNameExists(moveToAdd);

                if (charId > 0)
                {
                    string insertMoveSql = @"
                    INSERT INTO Move (Name, Image, Description, Motion) 
                    OUTPUT INSERTED.Id 
                    VALUES (@Name, @Image, @Description, @Motion);";

                    int newMoveId;
                    using (SqlCommand insertMoveCommand = new SqlCommand(insertMoveSql, (SqlConnection)connection))
                    {
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Name", moveToAdd.Name));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Image", moveToAdd.Image));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Description", moveToAdd.Description));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Motion", moveToAdd.Motion));

                        // Execute the command and get the inserted move's ID
                        newMoveId = (int)insertMoveCommand.ExecuteScalar();
                    }

                    string insertCharacterMoveSql = @"
                    INSERT INTO CharacterMove (Character_Id, Move_Id) 
                    VALUES (@Character_Id, @Move_Id);"
                            ;

                    using (SqlCommand insertCharacterMoveCommand = new SqlCommand(insertCharacterMoveSql, (SqlConnection)connection))
                    {
                        insertCharacterMoveCommand.Parameters.Add(new SqlParameter("@Character_Id", charId));
                        insertCharacterMoveCommand.Parameters.Add(new SqlParameter("@Move_Id", newMoveId));

                        insertCharacterMoveCommand.ExecuteNonQuery();
                    }
                }  
                else
                {
                    string insertMoveSql = @"
                    INSERT INTO Move (Name, Image, Description, Motion) 
                    VALUES (@Name, @Image, @Description, @Motion);";

                    int newMoveId;
                    using (SqlCommand insertMoveCommand = new SqlCommand(insertMoveSql, (SqlConnection)connection))
                    {
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Name", moveToAdd.Name));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Image", moveToAdd.Image));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Description", moveToAdd.Description));
                        insertMoveCommand.Parameters.Add(new SqlParameter("@Motion", moveToAdd.Motion));

                        insertMoveCommand.ExecuteNonQuery();
                    }
                }
               
            });
        }

        private MoveDto MapMoveDtoFromReader(SqlDataReader reader)
        {
            return new MoveDto
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                Motion = reader.IsDBNull(reader.GetOrdinal("Motion")) ? null : (string)reader["Motion"]             
            };
        }
    }
}
