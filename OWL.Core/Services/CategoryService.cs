using OWL.Core.CustomExceptions;
using OWL.Core.DTO;
using OWL.Core.Interfaces;
using OWL.Core.Logger;
using OWL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            this._categoryRepo = categoryRepository;
        }

        public Category GetCategoryById(int? categoryId)
        {
            if (!categoryId.HasValue)
            {
                throw new IdNotFoundException("Category ID has not been provided.");
            }

            CategoryDto categoryDto = _categoryRepo.GetCategoryDtoById(categoryId.Value);
            if (categoryDto == null)
            {
                throw new IdNotFoundException(categoryId.Value);
            }

            return new Category(categoryDto);
        }

        public List<Category> GetAllCategories()
        {
            try
            {
                List<CategoryDto> categoryDtos = _categoryRepo.GetAllCategories();
                return Category.MapToCategories(categoryDtos);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError("Error getting all fight styles", ex);
                throw;
            }
        }     

        public void AddCategory(Category category)
        {
            try
            {
                if (string.IsNullOrEmpty(category.Name))
                {
                    throw new NameRequiredException("Category name cannot be null or empty");
                }

                CategoryDto categoryDto = new CategoryDto(category);
                _categoryRepo.AddCategoryDto(categoryDto);
            }
            catch (Exception ex)
            {
                OwlLogger.LogError($"Error adding category {category.Name}", ex);
                throw;
            }
        }
    }
}
