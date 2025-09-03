using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface IPlayerRepository
    {
        Task<Player?> GetPlayerByUserId(int userId);
        Task<Player> CreatePlayer(int userId, string username);
        Task<Player> UpdatePlayerAsync(Player player);
    }
    public class PlayerRepository : IPlayerRepository
    {
        
        private readonly GameHubContext _db;
        public PlayerRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<Player?> GetPlayerByUserId(int userId)
        {
            return await _db.Players.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Player> CreatePlayer(int userId, string username)
        {
            var player = new Player
            {
                UserId = userId,
                Username = username,
                LastLogin = DateTime.Now
            };

            _db.Players.Add(player);
            await _db.SaveChangesAsync();
            return player;
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            var existingPlayer = await _db.Players.FindAsync(player.PlayerId);
            if (existingPlayer == null)
            {
                throw new InvalidOperationException("Player not found.");
            }
            existingPlayer.Username = player.Username;

            _db.Players.Update(existingPlayer);
            await _db.SaveChangesAsync();
            return existingPlayer;
        }
    }
}
