namespace ZocoApp.DTOs
{
    public class StudyDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Institution { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int UserId { get; set; }
    }
}

