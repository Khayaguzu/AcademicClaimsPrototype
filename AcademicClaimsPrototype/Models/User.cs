using System.ComponentModel.DataAnnotations;

namespace AcademicClaimsPrototype.Models
{
    public class User
    {
        [Key]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
    }
}