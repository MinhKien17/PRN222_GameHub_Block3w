using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using BusinessLogicLayer.Service;

namespace GameHub_DoanMinhKien_SE170383.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly IUserService _userService;
        public LogoutModel(IUserService userService)
        {
            _userService = userService;
        }
        public async Task OnPostAsync()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetById(userId);
            user.LastLogin = DateTime.Now;
            await _userService.UpdateUser(userId, user);

            await HttpContext.SignOutAsync("GameHubCookie");
            Response.Redirect("/");
        }
    }
}
