using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least 6 characters long", MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Username must be at least 3 characters long", MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = "Player";

        public string? ProfilePictureUrl { get; set; } = null;
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class UserDTO
    {
        public int? UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime JoinDate { get; set; }
        public string Role { get; set; }
        public string Username { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? newPassword { get; set; }
    }
}
