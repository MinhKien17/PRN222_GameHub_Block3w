using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Service;
using System.ComponentModel;

namespace GameHub_DoanMinhKien_SE170383.Pages.GameCategory
{
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (id == null)
            {
                return NotFound();
            }
            var existingCategory = await _categoryService.GetCategoryByIdAsync((int)id);

            if (existingCategory == null)
            {
                return NotFound();
            }
            await _categoryService.DeleteCategory((int)id);
            return RedirectToPage("/GameCategory/Index");
        }
    }
}
