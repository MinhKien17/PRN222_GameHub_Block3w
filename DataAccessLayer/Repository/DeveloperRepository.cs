using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface IDeveloperRepository
    {
        Task<Developer?> GetByIdAsync(int developerId);
        Task<List<Developer>> GetAllAsync();
    }
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly GameHubContext _db;
        public DeveloperRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<Developer?> GetByIdAsync(int developerId)
        {
            return await _db.Developers.FindAsync(developerId);
        }

        public async Task<List<Developer>> GetAllAsync()
        {
            return await _db.Developers.ToListAsync();
        }

    }
}
