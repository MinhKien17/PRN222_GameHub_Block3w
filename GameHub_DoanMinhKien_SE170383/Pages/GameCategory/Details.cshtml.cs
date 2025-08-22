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
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        public DetailsModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public CategoryDTO GameCategory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gamecategory = await _categoryService.GetCategoryByIdAsync((int)id);

            if (gamecategory is not null)
            {
                GameCategory = gamecategory;

                return Page();
            }

            return NotFound();
        }
    }
}
