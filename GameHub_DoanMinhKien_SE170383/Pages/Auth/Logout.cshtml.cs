using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameHub_DoanMinhKien_SE170383.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public async Task OnPostAsync()
        {
            await HttpContext.SignOutAsync("GameHubCookie");
            Response.Redirect("/");
        }
    }
}
