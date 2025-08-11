using System.ComponentModel.DataAnnotations;

namespace ZocoApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; 

        public ICollection<Study>? Studies { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<SessionLog>? SessionLogs { get; set; }
    }
}

