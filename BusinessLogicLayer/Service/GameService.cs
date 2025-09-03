using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Models;
using DataAccessLayer.Repository;
using X.PagedList;
using X.PagedList.Extensions;

namespace BusinessLogicLayer.Service
{
    public interface IGameService
    {
        Task<StaticPagedList<GameDTO>> GetAllGamesAsync(string? searchString, int pageNumber = 1, int pageSize = 10);
        Task<GameDTO> GetGameById(int gameId);
        Task<GameDTO> CreateGameAsync(GameDTO dto);
        Task<GameDTO> UpdateGameAsync(int gameId, GameDTO dto);
        Task DeleteGame(int gameId);
    }
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeveloperRepository _developerRepository;

        public GameService(IGameRepository gameRepository,
            IMediaRepository mediaRepository, IUserRepository userRepository,
            IDeveloperRepository developerRepository, ICategoryRepository categoryRepository)
        {
            _gameRepository = gameRepository;
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _developerRepository = developerRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<StaticPagedList<GameDTO>> GetAllGamesAsync(string? searchString, int pageNumber, int pageSize)
        {
            // Fetch paginated games from repository
            var (games, totalCounts) = await _gameRepository.GetAllGamesAsync(searchString, pageNumber, pageSize);

            // Map to GameDTO
            var gameDTOs = new List<GameDTO>();
            foreach (var game in games)
            {
                var gameDTO = new GameDTO
                {
                    GameId = game.GameId,
                    Title = game.Title,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    CategoryId = game.CategoryId ?? 0,
                    CategoryName = game.Category?.CategoryName ?? "N/A",
                    CategoryDescription = game.Category?.Description ?? "N/A",
                    DeveloperId = game.DeveloperId ?? 0,
                    DeveloperName = game.Developer?.DeveloperName ?? "N/A",
                    Website = game.Developer?.Website ?? "N/A",
                    Images = (await _mediaRepository.GetMediaByGameIdAsync(game.GameId, "Image"))
                        .Select(m => new MediaDTO
                        {
                            MediaId = m.MediaId,
                            FilePath = m.FilePath,
                            FileName = m.FileName,
                            Type = m.Type
                        }).ToList(),
                    Videos = (await _mediaRepository.GetMediaByGameIdAsync(game.GameId, "Video"))
                        .Select(m => new MediaDTO
                        {
                            MediaId = m.MediaId,
                            FilePath = m.FilePath,
                            FileName = m.FileName,
                            Type = m.Type
                        }).ToList()
                };
                gameDTOs.Add(gameDTO);
            }

            return new StaticPagedList<GameDTO>(gameDTOs, pageNumber, pageSize, totalCounts);
        }

        public async Task<GameDTO> GetGameById(int gameId)
        {
            var game = await _gameRepository.GetGameByIdAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync((int)game.CategoryId);
            var developer = await _developerRepository.GetByIdAsync((int)game.DeveloperId);

            var dto = new GameDTO
            {
                GameId = game.GameId,
                Title = game.Title,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                CategoryId = (int)game.CategoryId,
                DeveloperId = (int)game.DeveloperId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.Description,
                DeveloperName = developer.DeveloperName,
                Website = developer.Website
            };

            // Load images and videos
            dto.Images = (await _mediaRepository.GetMediaByGameIdAsync(game.GameId, "Image"))
                .Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            dto.Videos = (await _mediaRepository.GetMediaByGameIdAsync(game.GameId, "Video"))
                .Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            return dto;
        }

        public async Task<GameDTO> CreateGameAsync(GameDTO dto)
        {
            // Validate Category and Developer
            var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid CategoryId.");
            }

            var developer = await _developerRepository.GetByIdAsync(dto.DeveloperId);
            if (developer == null)
            {
                throw new ArgumentException("Invalid DeveloperId.");
            }

            // Map DTO to Entity
            var gameEntity = new Game
            {
                Title = dto.Title ?? throw new ArgumentException("Title is required."),
                Price = dto.Price ?? 0,
                ReleaseDate = dto.ReleaseDate,
                CategoryId = dto.CategoryId,
                DeveloperId = dto.DeveloperId,
                Category = category,
                Developer = developer,
            };

            // Save game
            gameEntity = await _gameRepository.CreateGameAsync(gameEntity);

            // Save images
            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                var savedImages = await _mediaRepository.SaveMediaAsync(gameEntity.GameId, dto.ImageFiles, "Image", dto.UploadById);
                dto.Images = savedImages.Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            }

            // Save videos
            if (dto.VideoFiles != null && dto.VideoFiles.Any())
            {
                var savedVideos = await _mediaRepository.SaveMediaAsync(gameEntity.GameId, dto.VideoFiles, "Video", dto.UploadById);
                dto.Videos = savedVideos.Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            }

            // Map back to DTO
            dto.GameId = gameEntity.GameId;
            dto.CategoryName = category.CategoryName;
            dto.CategoryDescription = category.Description;
            dto.DeveloperName = developer.DeveloperName;
            dto.Website = developer.Website;

            return dto;
        }

        public async Task<GameDTO> UpdateGameAsync(int gameId, GameDTO dto)
        {
            var game = await _gameRepository.GetGameByIdAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Invalid CategoryId.");
            }

            var developer = await _developerRepository.GetByIdAsync(dto.DeveloperId);
            if (developer == null)
            {
                throw new ArgumentException("Invalid DeveloperId.");
            }

            // Update game entity
            game.Title = dto.Title ?? throw new ArgumentException("Title is required.");
            game.Price = dto.Price ?? 0;
            game.ReleaseDate = dto.ReleaseDate;
            game.CategoryId = dto.CategoryId;
            game.DeveloperId = dto.DeveloperId;

            // Update game in repository
            await _gameRepository.UpdateGameAsync(game);

            // Remove media specified in RemoveMediaIds
            if (dto.RemoveMediaIds != null && dto.RemoveMediaIds.Any())
            {
                await _mediaRepository.DeleteMediaByGameIdAsync(gameId, dto.RemoveMediaIds);
            }

            // Save new image files
            if (dto.ImageFiles != null && dto.ImageFiles.Any())
            {
                var savedImages = await _mediaRepository.SaveMediaAsync(game.GameId, dto.ImageFiles, "Image", dto.UploadById);
                dto.Images = savedImages.Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            }
            else
            {
                dto.Images = (await _mediaRepository.GetMediaByGameIdAsync(gameId, "Image"))
                    .Select(m => new MediaDTO
                    {
                        MediaId = m.MediaId,
                        FilePath = m.FilePath,
                        FileName = m.FileName,
                        Type = m.Type
                    }).ToList();
            }

            // Save new video files
            if (dto.VideoFiles != null && dto.VideoFiles.Any())
            {
                var savedVideos = await _mediaRepository.SaveMediaAsync(game.GameId, dto.VideoFiles, "Video", dto.UploadById);
                dto.Videos = savedVideos.Select(m => new MediaDTO
                {
                    MediaId = m.MediaId,
                    FilePath = m.FilePath,
                    FileName = m.FileName,
                    Type = m.Type
                }).ToList();
            }
            else
            {
                dto.Videos = (await _mediaRepository.GetMediaByGameIdAsync(gameId, "Video"))
                    .Select(m => new MediaDTO
                    {
                        MediaId = m.MediaId,
                        FilePath = m.FilePath,
                        FileName = m.FileName,
                        Type = m.Type
                    }).ToList();
            }

            // Populate DTO with updated data
            dto.GameId = game.GameId;
            dto.CategoryName = category.CategoryName;
            dto.CategoryDescription = category.Description;
            dto.DeveloperName = developer.DeveloperName;
            dto.Website = developer.Website;

            return dto;
        }

        public async Task DeleteGame(int gameId)
        {
            var game = await _gameRepository.GetGameByIdAsync(gameId);
            var media = await _mediaRepository.GetMediaByGameIdAsync(gameId);
            var mediadIds = media.Select(m => m.MediaId).ToList();

            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }
            // Delete associated media
            await _mediaRepository.DeleteMediaByGameIdAsync(gameId, mediadIds);
            // Delete game
            await _gameRepository.DeleteGameAsync(gameId);
        }
    }
}
