using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Service;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameHub_DoanMinhKien_SE170383.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
        public RegisterModel(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }
        public void OnGet()
        {
        }
        [BindProperty]
        public RegisterDTO Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Input.ProfilePictureUrl))
            {
                var defaultPath = Path.Combine(_env.WebRootPath, "img", "user-default.jpg");
                if (System.IO.File.Exists(defaultPath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(defaultPath);

                    Input.ProfilePictureUrl = Convert.ToBase64String(bytes);
                }
            }

            try
            {
                var account = await _userService.Register(Input);

                if (account != null)
                {
                    return RedirectToPage("/Auth/Login");
                }
                ModelState.AddModelError("", "Registration failed. Please try again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }
            return Page();
        }
    }
}
