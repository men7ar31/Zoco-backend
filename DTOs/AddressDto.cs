using System.ComponentModel.DataAnnotations;

namespace ZocoApp.DTOs
{
    public class AddressDto
    {
        [Required, StringLength(120)] public string Street { get; set; } = string.Empty;
        [StringLength(80)] public string? City { get; set; }
        [StringLength(80)] public string? Country { get; set; }
    }
}
