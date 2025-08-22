using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.Repository
{
    public interface IMediaRepository
    {
        Task<List<Media>> GetAllAsync();
        Task<Media?> GetByUserId(int userId);
        Task<Media> CreateUserImg(int userId, byte[] file);
        Task<Media> UploadForUser(int userId, byte[] file);
        Task<Media> UpdateUserImg(int userId, byte[] file);
        Task<List<Media>> SaveMediaAsync(int gameId, IEnumerable<IFormFile> files, string type, int uploadById);
        Task<List<Media>> GetMediaByGameIdAsync(int gameId, string? type = null);
        Task DeleteMediaByGameIdAsync(int gameId, List<int> mediaIdsToDelete);
    }
    public class MediaRepository : IMediaRepository
    {
        private readonly GameHubContext _db;
        private readonly IHostingEnvironment _env;
        public MediaRepository(GameHubContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<List<Media>> GetAllAsync()
        {
            return await _db.Media.ToListAsync();
        }

        // Involve User Img
        public async Task<Media?> GetByUserId(int userId)
        {
            return await _db.Media.FirstOrDefaultAsync(m => m.UserId == userId);
        }
        public async Task<Media> UploadForUser(int userId, byte[] file)
        {
            var media = new Media
            {
                UserId = userId,
                Data = file,
                UploadedAt = DateTime.Now,
                UploadById = userId,
            };
            _db.Media.Add(media);
            await _db.SaveChangesAsync();
            return media;
        }
        public async Task<Media> CreateUserImg(int userId, byte[] file)
        {
            var media = new Media
            {
                UserId = userId,
                Data = file,
                UploadedAt = DateTime.Now,
                UploadById = userId,
                Type = "image/jpeg"
            };
            _db.Media.Add(media);
            await _db.SaveChangesAsync();
            return media;
        }
        public async Task<Media> UpdateUserImg(int userId, byte[] file)
        {
            var existingMedia = await _db.Media.FirstOrDefaultAsync(m => m.UserId == userId);
            if (existingMedia == null)
            {
                throw new InvalidOperationException("Media not found for this user.");
            }

            existingMedia.Data = file;
            existingMedia.UploadedAt = DateTime.Now;
            existingMedia.UploadById = userId;
            existingMedia.Type = "image/jpeg";

            await _db.SaveChangesAsync();
            return existingMedia;
        }

        // Invole Game Videos and Imgs
        public async Task<List<Media>> SaveMediaAsync(int gameId, IEnumerable<IFormFile> files, string type, int uploadById)
        {
            var savedMedia = new List<Media>();

            // Ensure uploads directory exists
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Validate file type
                    if (!IsValidFileType(file, type))
                    {
                        throw new InvalidOperationException($"Invalid file type for {type}. Only images or videos are allowed.");
                    }

                    // Generate unique file name
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);

                    // Save file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Create Media 
                    var media = new Media
                    {
                        GameId = gameId,
                        FileName = file.FileName, 
                        FilePath = $"/uploads/{uniqueFileName}",
                        Type = type,
                        UploadedAt = DateTime.Now,
                        UploadById = uploadById,
                        Data = null
                    };

                    _db.Media.Add(media);
                    savedMedia.Add(media);
                }
            }

            await _db.SaveChangesAsync();
            return savedMedia;
        }

        public async Task<List<Media>> GetMediaByGameIdAsync(int gameId, string? type = null)
        {
            var query = _db.Media.Where(m => m.GameId == gameId);
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(m => m.Type == type);
            }
            return await query.ToListAsync();
        }

        public async Task DeleteMediaByGameIdAsync(int gameId, List<int> mediaIdsToDelete)
        {
            var mediaList = await _db.Media
                .Where(m => m.GameId == gameId && mediaIdsToDelete.Contains(m.MediaId))
                .ToListAsync();

            foreach (var media in mediaList)
            {
                if (!string.IsNullOrEmpty(media.FilePath))
                {
                    var relativePath = media.FilePath.TrimStart('/', '\\').Replace("..", "");
                    var filePath = Path.Combine(_env.WebRootPath, relativePath);

                    if (File.Exists(filePath))
                    {
                        int retries = 3;
                        int delayMs = 500;
                        while (retries > 0)
                        {
                            try
                            {
                                File.Delete(filePath);
                                break;
                            }
                            catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                            {
                                retries--;
                                if (retries == 0)
                                {
                                    throw new Exception($"Failed to delete file {filePath} after retries: {ex.Message}");
                                }
                                await Task.Delay(delayMs);
                            }
                        }
                    }
                }

                _db.Media.Remove(media);
            }

            await _db.SaveChangesAsync();
        }

        // Helper Method
        private bool IsValidFileType(IFormFile file, string type)
        {
            var contentType = file.ContentType.ToLower();
            if (type == "Image")
            {
                return contentType.StartsWith("image/");
            }
            else if (type == "Video")
            {
                return contentType.StartsWith("video/");
            }
            return false;
        }
    }
}
