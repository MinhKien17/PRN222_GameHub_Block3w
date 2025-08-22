using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameHub_DoanMinhKien_SE170383.Pages.Auth
{
    public class AccessDeniedModel : PageModel
    {
        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; } = "";
        public string Role { get; private set; } = "";

        public void OnGet()
        {
            IsAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            if (IsAuthenticated)
            {
                Name = User.Identity?.Name ?? "";
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            }
        }
    }
}
