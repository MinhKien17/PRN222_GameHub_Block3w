using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllGamesAsync();
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

        public async Task<List<Game>> GetAllGamesAsync()
        {
            return await _db.Games.ToListAsync();
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
