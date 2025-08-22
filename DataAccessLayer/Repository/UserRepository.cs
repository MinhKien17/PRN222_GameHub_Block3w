using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public interface IUserRepository
    {
        Task<User> Register(string email, string password, string role);
        Task<User?> Login(string email, string password);
        Task<User?> GetByIdAsync(int userId);
        Task<User> UpdateAsync(User user, string? newPassword = null);
    }
    public class UserRepository : IUserRepository
    {
        private readonly GameHubContext _db;
        public UserRepository(GameHubContext db)
        {
            _db = db;
        }

        public async Task<User> Register(string email, string password, string role)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = new User
            {
                Email = email,
                JoinDate = DateTime.Now,
                Role = role
            };

            var hash = new PasswordHasher<User>();
            user.PasswordHash = hash.HashPassword(user, password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null) return null;

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _db.Users.FindAsync(userId);
        }

        public async Task<User> UpdateAsync(User user, string? newPassword)
        {
            var existingUser = await _db.Users.FindAsync(user.UserId);
            if (existingUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            existingUser.Email = user.Email;
            existingUser.Role = user.Role;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                var hasher = new PasswordHasher<User>();
                existingUser.PasswordHash = hasher.HashPassword(existingUser, newPassword);
            }

            _db.Users.Update(existingUser);
            await _db.SaveChangesAsync();
            return existingUser;
        }
    }
}
