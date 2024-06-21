using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OWL.Core.Interfaces;
using OWL.Core.DTO;
using OWL.Core.CustomExceptions;
using OWL.DataAccess.DB;
using System.Data.SqlClient;

namespace OWL.DataAccess.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DatabaseConnection databaseConnection;

        public CategoryRepository(DatabaseConnection databaseConnection)
        {
            this.databaseConnection = databaseConnection;
        }

        public CategoryDto GetCategoryDtoById(int categoryId)
        {
            CategoryDto result = null;

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Id, Name FROM Category WHERE Id = @Id;";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", categoryId));

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
                                result = MapCategoryDtoFromReader(reader);
                            }
                        }
                    }
                }
            });

            return result;
        }

        public void AddCategoryDto(CategoryDto categoryToAdd)
        {
            databaseConnection.StartConnection(connection =>
            {
                // First, check if the Name already exists in the database
                string checkSql = "SELECT COUNT(*) FROM Category WHERE Name = @Name;";
                using (SqlCommand checkCommand = new SqlCommand(checkSql, (SqlConnection)connection))
                {
                    checkCommand.Parameters.Add(new SqlParameter("@Name", categoryToAdd.Name));
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        // Name already exists, handle the error
                        throw new NameExistsException("A category with this name already exists.", categoryToAdd.Name);
                    }
                }

                // If the name does not exist, proceed with the insertion
                string insertSql = "INSERT INTO Category (Name) VALUES (@Name);";
                using (SqlCommand insertCommand = new SqlCommand(insertSql, (SqlConnection)connection))
                {
                    insertCommand.Parameters.Add(new SqlParameter("@Name", categoryToAdd.Name));

                    insertCommand.ExecuteNonQuery();
                }
            });
        }

        public List<CategoryDto> GetAllCategories()
        {
            List<CategoryDto> categories = new List<CategoryDto>();

            databaseConnection.StartConnection(connection =>
            {
                string sql = "SELECT Id, Name FROM category";
                using (SqlCommand command = new SqlCommand(sql, (SqlConnection)connection))

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(MapCategoryDtoFromReader(reader));
                    }
                }
            });

            return categories;
        }

        private CategoryDto MapCategoryDtoFromReader(SqlDataReader reader)
        {
            return new CategoryDto
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"]
            };
        }
    }
}
