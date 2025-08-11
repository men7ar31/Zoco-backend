namespace ZocoApp.DTOs
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

    }
}
