using OWL.DataAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.DTO;
using OWL.Core.CustomExceptions;
using System.Data.SqlClient;
using OWL.Core.Models;

namespace OWL.DataAccess.Repository
{
    public class FightstyleRepository : IFightstyleRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public FightstyleRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public FightstyleDto GetFightstyleDtoById(int styleId)
        {
            FightstyleDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Id, Name, Power, Speed FROM Fightstyle WHERE Id = @Id;";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", styleId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.IsDBNull(0))
                            {
                                throw new IdNotFoundException(reader.GetInt32(0));
                            }
                            else
                            {
                                result = MapFightstyleDtoFromReader(reader);
                            }
                        }
                    }
                }
            });

            return result;
        }

        public void AddFightstyleDto(FightstyleDto styleToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checkSql = "SELECT COUNT(*) FROM Fightstyle WHERE Name = @Name;";
                using (SqlCommand checkCommand = new SqlCommand(checkSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Name", styleToAdd.Name));
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("A fightstyle with this name already exists.", styleToAdd.Name);
                    }
                }

                // If the name does not exist, proceed with the insertion
                string insertSql = "INSERT INTO Fightstyle (Name, Power, Speed) VALUES (@Name, @Power, @Speed);";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@Name", styleToAdd.Name));
                    insertCommand.Parameters.Add(new SqlParameter("@Power", styleToAdd.Power));
                    insertCommand.Parameters.Add(new SqlParameter("@Speed", styleToAdd.Speed));

                    insertCommand.ExecuteNonQuery();
                }
            });
        }

        public List<FightstyleDto> GetAllFightstylesNotMatchingCharacter()
        {
            List<FightstyleDto> styles = new List<FightstyleDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
            SELECT fs.Id, fs.Name, fs.Power, fs.Speed
            FROM Fightstyle fs
            LEFT JOIN Character c ON fs.Id = c.fightstyle_id
            WHERE c.fightstyle_id IS NULL OR c.Id IS NULL;
                 ";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        styles.Add(MapFightstyleDtoFromReader(reader));
                    }
                }
            });

            return styles;
        }



        public List<FightstyleDto> GetAllFightstyles()
        {
            List<FightstyleDto> styles = new List<FightstyleDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Id, Name, Power, Speed FROM Fightstyle";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        styles.Add(MapFightstyleDtoFromReader(reader));
                    }
                }
            });

            return styles;
        }

        private FightstyleDto MapFightstyleDtoFromReader(SqlDataReader reader)
        {
            return new FightstyleDto
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Power = (int)reader["Power"],
                Speed = (int)reader["Speed"]
                
            };
        }


    }
}
