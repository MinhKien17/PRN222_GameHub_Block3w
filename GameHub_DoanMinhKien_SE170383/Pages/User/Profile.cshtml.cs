using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameHub_DoanMinhKien_SE170383.Pages.User
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        public ProfileModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public UserDTO UserProfile { get; set; } = new UserDTO();

        public async Task OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = await _userService.GetById(userId);

                if (user != null)
                {
                    UserProfile = user;
                    UserProfile.Role = user.Role;
                    UserProfile.PasswordHash = user.PasswordHash;
                }
                else
                {
                    ViewData["ErrorMessage"] = "User not found.";
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "You must be logged in to view your profile.";
            }
        }

        public async Task<IActionResult> OnPostAsync(IFormFile? avatarFile)
        {
            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Model binding error in {entry.Key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            } 

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingUser = await _userService.GetById(userId);

            try
            {
                if (avatarFile != null && avatarFile.Length > 0)
                {
                    if (!avatarFile.ContentType.StartsWith("image/"))
                    {
                        TempData["Error"] = "File must be an image.";
                        return RedirectToPage();
                    }

                    const long maxSize = 100 * 1024 * 1024; // 10MB
                    if (avatarFile.Length > maxSize)
                    {
                        TempData["Error"] = "File too large. Max 2MB.";
                        return RedirectToPage();
                    }

                    await using var ms = new MemoryStream();
                    await avatarFile.CopyToAsync(ms);
                    var bytes = ms.ToArray();
                    var base64 = Convert.ToBase64String(bytes);

                    UserProfile.ProfilePictureUrl = $"data:{avatarFile.ContentType};base64,{base64}";
                }

                if (string.IsNullOrEmpty(UserProfile.PasswordHash))
                {
                    UserProfile.PasswordHash = existingUser.PasswordHash;
                }

                await _userService.UpdateUser(userId, UserProfile);

                TempData["Success"] = avatarFile != null ? "Profile and avatar updated." : "Profile updated.";

                // Update claims
                var currentClaims = User.Claims.ToList();
                currentClaims.RemoveAll(c => c.Type == ClaimTypes.Name);
                currentClaims.Add(new Claim(ClaimTypes.Name, UserProfile.Username));
                currentClaims.Add(new Claim(ClaimTypes.Email, UserProfile.Email));

                var identity = new ClaimsIdentity(currentClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("GameHubCookie", principal);

                return RedirectToPage("");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }

    }
}
