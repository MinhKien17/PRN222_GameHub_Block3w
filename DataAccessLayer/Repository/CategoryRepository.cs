using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface ICategoryRepository
    {
        Task<List<GameCategory>> GetAllCategoriesAsync();
        Task<GameCategory?> GetCategoryByIdAsync(int categoryId);
        Task<GameCategory> CreateCategoryAsync(GameCategory category);
        Task<GameCategory> UpdateCategoryAsync(GameCategory category);
        Task DeleteCategoryAsync(int categoryId);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly GameHubContext _db;
        public CategoryRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<List<GameCategory>> GetAllCategoriesAsync()
        {
            return await _db.GameCategories.ToListAsync();
        }
        public async Task<GameCategory?> GetCategoryByIdAsync(int categoryId)
        {
            return await _db.GameCategories.FindAsync(categoryId);
        }

        public async Task<GameCategory> CreateCategoryAsync(GameCategory category)
        {
            _db.GameCategories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<GameCategory> UpdateCategoryAsync(GameCategory category)
        {
            _db.GameCategories.Update(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _db.GameCategories.FindAsync(categoryId);
            if (category != null)
            {
                _db.GameCategories.Remove(category);
                await _db.SaveChangesAsync();
            }
        }
    }
}
