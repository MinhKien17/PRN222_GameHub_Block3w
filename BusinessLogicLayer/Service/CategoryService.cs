using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;

namespace BusinessLogicLayer.Service
{
    public interface ICategoryService
    {
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId);
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto);
        Task<CategoryDTO> UpdateCategory(CategoryDTO dto);
        Task DeleteCategory(int categoryId);

    }
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGameRepository _gameRepository;

        public CategoryService(ICategoryRepository categoryRepository, IGameRepository gameRepository)
        {
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
        }
        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();
        }

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            if (category == null) return null;
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var category = new GameCategory
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description
            };
            await _categoryRepository.CreateCategoryAsync(category);
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<CategoryDTO> UpdateCategory(CategoryDTO dto)
        {
            var (games, totalCount) = await _gameRepository.GetAllGamesAsync(null, 1,int.MaxValue);
            foreach (var check in games)
            {
                if (check.CategoryId == dto.CategoryId)
                {
                    throw new InvalidOperationException("Cannot update category as it is associated with existing games.");
                }
            }

            var category = new GameCategory
            {
                CategoryId = dto.CategoryId ?? 0,
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };
            await _categoryRepository.UpdateCategoryAsync(category);
            return new CategoryDTO
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task DeleteCategory(int categoryId)
        {
            var (games, totalCount) = await _gameRepository.GetAllGamesAsync(null, 1, int.MaxValue);
            if (games.Any(g => g.CategoryId == categoryId))
            {
                throw new InvalidOperationException("Cannot delete category as it is associated with existing games.");
            }

            await _categoryRepository.DeleteCategoryAsync(categoryId);
        }
    }
}
