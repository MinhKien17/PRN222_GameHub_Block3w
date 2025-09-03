using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface IPlayerLibaryRepository
    {
        Task<List<PlayerLibrary>?> GetLibraryByPlayerId(int playerId);
        Task<PlayerLibrary?> AddGameToLibrary(int playerId, int gameId);
        Task<bool> RemoveGameFromLibrary(int playerId, int gameId);
    }
    public class PlayerLibaryRepository : IPlayerLibaryRepository
    {
        private readonly GameHubContext _db;
        public PlayerLibaryRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<List<PlayerLibrary>?> GetLibraryByPlayerId(int playerId)
        {
            return await _db.PlayerLibraries.Where(pl => pl.PlayerId == playerId).ToListAsync();
        }

        public async Task<PlayerLibrary?> AddGameToLibrary(int playerId, int gameId)
        {
            var game = await _db.Games.FirstOrDefaultAsync(g => g.GameId == gameId);
            var player = await _db.Players.FirstOrDefaultAsync(u => u.PlayerId == playerId);
            if (game == null || player == null)
            {
                throw new InvalidOperationException("Game or User not found.");
            }

            var playerLibrary = new PlayerLibrary
            {
                PlayerId = playerId,
                GameId = gameId,
                Status = "Not Started",
                TotalPlayMinutes = 0,
                IsFavorite = false,
                Notes = string.Empty,
                Game = game,
                Player = player
            };

            _db.PlayerLibraries.Add(playerLibrary);
            await _db.SaveChangesAsync();
            return playerLibrary;
        }

        public async Task<bool> RemoveGameFromLibrary(int playerId, int gameId)
        {
            var playerLibrary = await _db.PlayerLibraries
                .FirstOrDefaultAsync(pl => pl.PlayerId == playerId && pl.GameId == gameId);
            if (playerLibrary == null)
            {
                return false; // Game not found in library
            }
            _db.PlayerLibraries.Remove(playerLibrary);
            await _db.SaveChangesAsync();
            return true;
        }

    }
}
