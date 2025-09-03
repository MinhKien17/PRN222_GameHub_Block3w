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
using X.PagedList;

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
        public StaticPagedList<GameDTO> Game { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? searchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int pageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 10;
        public async Task OnGetAsync()
        {
            try
            {
                Game = await _gameService.GetAllGamesAsync(searchString, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine(ex.Message);
                // Handle the error appropriately (e.g., show an error message to the user)
                ModelState.AddModelError(string.Empty, "An error occurred while loading the games.");
            }
        }
    }
}
