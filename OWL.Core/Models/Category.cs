using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class Category
    {
        public int Id { get;  set; }

        public string Name { get;  set; }

        public Category() { }

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Category(CategoryDto categoryDto)
        {
            Id = categoryDto.Id;
            Name = categoryDto.Name;
        }

        public static List<Category> MapToCategories(List<CategoryDto> categoryDtos)
        {

            List<Category> categories = new List<Category>();

            try
            {
                foreach (CategoryDto categoryDto in categoryDtos)
                {
                    categories.Add(new Category(categoryDto));
                }
            }
            catch (Exception ex)
            {

            }


            return categories;
        }
    }
}
