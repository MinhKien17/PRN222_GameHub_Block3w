using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;
using BusinessLogicLayer.Service;
using BusinessLogicLayer.DTO;
using Microsoft.AspNetCore.Authorization;

namespace GameHub_DoanMinhKien_SE170383.Pages.Games
{
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IGameService _gameService;
        public DeleteModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty]
        public GameDTO Game { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameService.GetGameById((int)id);

            if (game is not null)
            {
                Game = game;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameService.GetGameById((int)id);
            if (game != null)
            {
                await _gameService.DeleteGame((int)id);
            }

            return RedirectToPage("./Index");
        }
    }
}
