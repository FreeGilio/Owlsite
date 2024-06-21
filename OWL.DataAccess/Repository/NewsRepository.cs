using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using OWL.Core.CustomExceptions;
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

        public NewsDto GetNewsDtoById(int newsId)
        {
            NewsDto result = null;

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
                    Category c ON n.Category_id = c.Id
                WHERE 
                    n.Id = @Id;";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", newsId));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (string.IsNullOrEmpty(reader.GetString(1)))
                            {
                                throw new NewsNotFoundException(reader.GetString(1));
                            }
                            else
                            {
                                result = MapNewsDtoFromReader(reader);
                            }
                        }
                    }
                }
            });

            return result;
        }

        public bool CheckTitleExists(NewsDto news, int? currentNewsId = null)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checknameSql = "SELECT COUNT(*) FROM News WHERE Title = @Title";

                if (currentNewsId.HasValue)
                {
                    checknameSql += " AND Id != @CurrentNewsId";
                }

                using (SqlCommand checkCommand = new SqlCommand(checknameSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Title", news.Title));

                    if (currentNewsId.HasValue)
                    {
                        checkCommand.Parameters.Add(new SqlParameter("@CurrentNewsId", currentNewsId.Value));
                    }

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("An article with this title already exists.", news.Title);
                    }
                }
            });
            return false;
        }

        public List<NewsDto> GetAllNewsWithCategories()
        {
            List<NewsDto> news = new List<NewsDto>();

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
                        news.Add(MapNewsDtoFromReader(reader));
                    }
                }
            });

            // Order news articles by Date in descending order (latest first)
            news = news.OrderByDescending(n => n.Date).ToList();

            return news;
        }

        public void AddNewsDto(NewsDto newsToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {

                CheckTitleExists(newsToAdd);

                string insertSql = "INSERT INTO News (Title, Image, Description, Date, Category_id) VALUES (@Title, @Image, @Description, @Date, @Category_id);";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@Title", newsToAdd.Title));
                    insertCommand.Parameters.Add(new SqlParameter("@Image", newsToAdd.Image));
                    insertCommand.Parameters.Add(new SqlParameter("@Description", newsToAdd.Description));
                    insertCommand.Parameters.Add(new SqlParameter("@Date", DateTime.Now));
                    insertCommand.Parameters.Add(new SqlParameter("@Category_id", newsToAdd.Category.Id));

                    insertCommand.ExecuteNonQuery();

                }
            });
        }

        public void UpdateNewsDto(NewsDto newsToUpdate)
        {
            databaseConnection.StartConnection(connection =>
            {
                CheckTitleExists(newsToUpdate, newsToUpdate.Id);

                string updateSql = "UPDATE News SET Title = @Title, Image = @Image, Description = @Description, Date = @Date, Category_id = @Category_id WHERE Id = @Id;";
                using (SqlCommand updateCommand = new SqlCommand(updateSql, (SqlConnection)connection))
                {
                    updateCommand.Parameters.Add(new SqlParameter("@Id", newsToUpdate.Id));
                    updateCommand.Parameters.Add(new SqlParameter("@Title", newsToUpdate.Title));
                    updateCommand.Parameters.Add(new SqlParameter("@Image", newsToUpdate.Image));
                    updateCommand.Parameters.Add(new SqlParameter("@Description", newsToUpdate.Description));
                    updateCommand.Parameters.Add(new SqlParameter("@Date", newsToUpdate.Date.ToDateTime(TimeOnly.MinValue)));
                    updateCommand.Parameters.Add(new SqlParameter("@Category_id", newsToUpdate.Category.Id));

                    updateCommand.ExecuteNonQuery();

                }
            });
        }

        public void DeleteNews(NewsDto newsDto)
        {
            databaseConnection.StartConnection(connection =>
            {

                // Delete the character
                string deleteSql = "DELETE FROM News WHERE Id = @Id;";
                using (SqlCommand deleteCommand = new SqlCommand(deleteSql, (SqlConnection)connection))
                {
                    deleteCommand.Parameters.Add(new SqlParameter("@Id", newsDto.Id));
                    deleteCommand.ExecuteNonQuery();
                }

                // Optionally reset the auto-increment value if required
                // This step is usually not necessary as SQL Server handles it automatically
                string resetAutoIncrementSql = @"
                    DECLARE @MaxId INT;
                    SELECT @MaxId = ISNULL(MAX(Id), 0) FROM News;
                    DBCC CHECKIDENT ('News', RESEED, @MaxId);
                ";
                using (SqlCommand resetCommand = new SqlCommand(resetAutoIncrementSql, (SqlConnection)connection))
                {
                    resetCommand.ExecuteNonQuery();
                }
            });
        }

        private NewsDto MapNewsDtoFromReader(SqlDataReader reader)
        {
            return new NewsDto
            {
                Id = (int)reader["Id"],
                Title = (string)reader["Title"],
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : (string)reader["Description"],
                Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (string)reader["Image"],
                Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? DateOnly.MinValue : DateOnly.FromDateTime((DateTime)reader["Date"]),
                Category = new Category
                {
                    Id = (int)reader["CategoryId"],
                    Name = (string)reader["Name"]
                }
            };
        }


    }
}
