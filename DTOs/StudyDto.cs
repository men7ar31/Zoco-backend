using System.ComponentModel.DataAnnotations;

namespace ZocoApp.DTOs
{
    public class StudyDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        public string? Institution { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

