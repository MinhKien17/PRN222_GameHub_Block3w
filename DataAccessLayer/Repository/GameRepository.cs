using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace DataAccessLayer.Repository
{
    public interface IGameRepository
    {
        Task<(IEnumerable<Game>, int)> GetAllGamesAsync(string? searchString, int pageNumber = 1, int pageSize = 10);
        Task<Game?> GetGameByIdAsync(int gameId);
        Task<Game> CreateGameAsync(Game game);
        Task<Game> UpdateGameAsync(Game game);
        Task DeleteGameAsync(int gameId);
    }
    public class GameRepository : IGameRepository
    {
        private readonly GameHubContext _db;
        public GameRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Game>, int)> GetAllGamesAsync(string? searchString, int pageNumber, int pageSize)
        {
            var query = _db.Games
                    .Include(g => g.Category)
                    .Include(g => g.Developer)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                string searchLower = searchString.Trim().ToLower();

                bool isPriceSearch = decimal.TryParse(searchLower, out decimal searchPrice);

                query = query.Where(g =>
                    // Title search
                    g.Title.ToLower().Contains(searchLower) ||
                    // CategoryName search (with null check)
                    (g.Category != null && g.Category.CategoryName != null && g.Category.CategoryName.ToLower().Contains(searchLower)) ||
                    // Price search (exact match if searchString is a valid decimal)
                    (isPriceSearch && g.Price == searchPrice) ||
                    // DeveloperName search (with null check)
                    (g.Developer != null && g.Developer.DeveloperName != null && g.Developer.DeveloperName.ToLower().Contains(searchLower)) 
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Fetch paginated data
            var games = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(g => g.Category)
                .Include(g => g.Developer)
                .ToListAsync();

            return (games, totalCount);
        }

        public async Task<Game?> GetGameByIdAsync(int gameId)
        {
            return await _db.Games.FindAsync(gameId);
        }
        public async Task<Game> CreateGameAsync(Game game)
        {
            _db.Games.Add(game);
            await _db.SaveChangesAsync();
            return game;
        }

        public async Task<Game> UpdateGameAsync(Game game)
        {
            _db.Games.Update(game);
            await _db.SaveChangesAsync();
            return game;
        }
        public async Task DeleteGameAsync(int gameId)
        {
            var game = await GetGameByIdAsync(gameId);
            if (game != null)
            {
                _db.Games.Remove(game);
                await _db.SaveChangesAsync();
            }
        }

    }
}
