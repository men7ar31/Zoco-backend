using System.ComponentModel.DataAnnotations;

namespace ZocoApp.DTOs
{
    public class MeUpdateDto
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        public string? NewPassword { get; set; }
    }
}

