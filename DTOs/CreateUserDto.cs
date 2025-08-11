using System.ComponentModel.DataAnnotations;

namespace ZocoApp.DTOs
{
    public class CreateUserDto
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
}

