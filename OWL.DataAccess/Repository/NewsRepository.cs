using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using OWL.Core.DTO;
using OWL.Core.Interfaces;
using OWL.Core.Models;
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

        public List<NewsDto> GetAllNewsWithCategories()
        {
            List<NewsDto> characters = new List<NewsDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = @"
                SELECT 
                    n.Id as Id,
                    n.Title,
                    n.Description,
                    n.Image,
                    n.Date,
                    c.Id as CategoryId,
                    c.Name
                FROM 
                    News n
                JOIN
                    Category c ON n.Category_id = c.Id";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        characters.Add(MapNewsDtoFromReader(reader));
                    }
                }
            });

            return characters;
        }

        private NewsDto MapNewsDtoFromReader(SqlDataReader reader)
        {
            return new NewsDto
            {
                Id = (int)reader["Id"],
                Title = (string)reader["Title"],
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                Date = (DateTime)reader["Date"],
                Category = new Category
                {
                    Id = (int)reader["CategoryId"],
                    Name = (string)reader["Name"]
                }
            };
        }
    }
}
