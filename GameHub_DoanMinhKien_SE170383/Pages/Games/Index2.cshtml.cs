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
    [Authorize(Roles = "Player")]
    public class IndexModel2 : PageModel
    {
        private readonly IGameService _gameService;
        public IndexModel2(IGameService gameService)
        {
            _gameService = gameService;
        }
        public List<GameDTO> Game { get; set; } = default!;

        public async Task OnGetAsync()
        {
            try
            {
                Game = await _gameService.GetAllGamesAsync();
            }
            catch (Exception ex)
            {
                Game = new List<GameDTO>(); // Ensure Game is initialized even on error
            }
        }
    }
}
