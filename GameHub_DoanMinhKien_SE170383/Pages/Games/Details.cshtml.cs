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
    public class DetailsModel : PageModel
    {
        private readonly IGameService _gameService;
        public DetailsModel(IGameService gameService)
        {
            _gameService = gameService;
        }
        public GameDTO Game { get; set; } = new GameDTO();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Game = await _gameService.GetGameById(id);
            if (Game == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
