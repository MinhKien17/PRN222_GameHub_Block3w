using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.Models;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Service;
using DataAccessLayer.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GameHub_DoanMinhKien_SE170383.Pages.Games
{
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class CreateModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeveloperRepository _developerRepository;

        public CreateModel(IGameService gameService, 
            ICategoryRepository categoryRepository, IDeveloperRepository developerRepository)
        {
            _gameService = gameService;
            _categoryRepository = categoryRepository;
            _developerRepository = developerRepository;
        }

        [BindProperty]
        public GameDTO Game { get; set; } = new GameDTO();

        public SelectList Categories { get; set; }
        public SelectList Developers { get; set; }

        public async Task OnGetAsync()
        {
            Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
            Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
                Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName");
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Game.UploadById = userId != null ? int.Parse(userId) : 0;

            try
            {
                await _gameService.CreateGameAsync(Game);
                TempData["Success"] = "Game created successfully!";
                return RedirectToPage("/Games/Details", new { id = Game.GameId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
                Developers = new SelectList(await _developerRepository.GetAllAsync(), "DeveloperId", "DeveloperName");
                return Page();
            }
        }
    }
}
