using System.ComponentModel.DataAnnotations;

namespace ZocoApp.DTOs
{
    public class UpdateUserDto
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? NewPassword { get; set; } 
    }
}
