using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CategoryDto() { }

        public CategoryDto(Category category)
        {
            Id = category.Id;
            Name = category.Name;
        }
    }
}
