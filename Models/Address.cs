using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZocoApp.Models
{
    public class Address
    {
        public int Id { get; set; }

        [Required]
        public string Street { get; set; } = string.Empty;

        public string? City { get; set; }
        public string? Country { get; set; }

        
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
