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

namespace GameHub_DoanMinhKien_SE170383.Pages.GameCategory
{
    [Authorize(Roles ="Staff, Developer, Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<CategoryDTO> GameCategory { get;set; } = default!;

        public async Task OnGetAsync()
        {
            GameCategory = await _categoryService.GetAllCategoriesAsync();
        }
    }
}
