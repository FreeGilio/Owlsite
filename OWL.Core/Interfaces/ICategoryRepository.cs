using OWL.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Interfaces
{
    public interface ICategoryRepository
    {
        CategoryDto GetCategoryDtoById(int categoryId);

        List<CategoryDto> GetAllCategories();
        void AddCategoryDto(CategoryDto categoryToAdd);
    }
}
