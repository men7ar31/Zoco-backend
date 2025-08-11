using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZocoApp.Models
{
    public class Study
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Institution { get; set; }

        public DateTime? CompletedAt { get; set; }

       
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
