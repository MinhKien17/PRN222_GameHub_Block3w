using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;
using BusinessLogicLayer.Service;
using DataAccessLayer.Repository;
using BusinessLogicLayer.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GameHub_DoanMinhKien_SE170383.Pages.Games
{
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class EditModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeveloperRepository _developerRepository;
        private readonly ILogger<EditModel> _logger;
        public EditModel(IGameService gameService, 
            ICategoryRepository categoryRepository, IDeveloperRepository developerRepository, ILogger<EditModel> logger)
        {
            _gameService = gameService;
            _categoryRepository = categoryRepository;
            _developerRepository = developerRepository;
            _logger = logger;
        }

        [BindProperty]
        public GameDTO Game { get; set; }

        public SelectList Categories { get; set; }
        public SelectList Developers { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var game = await _gameService.GetGameById(id);
            if (game == null)
            {
                return NotFound();
            }

            Game = game;
            Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName", Game.CategoryId);
            Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName", Game.DeveloperId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName", Game.CategoryId);
                Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName", Game.DeveloperId);
                return Page();
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Game.UploadById = int.Parse(userId);

                await _gameService.UpdateGameAsync(Game.GameId.Value, Game);
                TempData["Success"] = "Game updated successfully!";
                return RedirectToPage("/Games/Details", new { id = Game.GameId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating game: {Message}", ex.Message);
                ModelState.AddModelError("", ex.Message);
                Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName", Game.CategoryId);
                Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName", Game.DeveloperId);
                return Page();
            }
        }

    }
}
