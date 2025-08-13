using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZocoApp.Models
{
    public class SessionLog
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        // Opcional: datos de contexto
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
