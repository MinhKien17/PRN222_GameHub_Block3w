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
using BusinessLogicLayer.DTO;
using Microsoft.AspNetCore.Authorization;

namespace GameHub_DoanMinhKien_SE170383.Pages.GameCategory
{
    [Authorize(Roles = "Developer, Staff, Admin")]
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        public EditModel(ICategoryService categoryService)
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

            var gamecategory =  await _categoryService.GetCategoryByIdAsync((int)id);
            if (gamecategory is not null)
            {
                GameCategory = gamecategory;
                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _categoryService.DeleteCategory((int)GameCategory.CategoryId);
                return RedirectToPage("/GameCategory/Index");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}, StackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while deleting the category.");
                return Page();
            }
        }
    }
}
