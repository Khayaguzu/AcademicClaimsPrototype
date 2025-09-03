using System.ComponentModel.DataAnnotations;


namespace AcademicClaimsPrototype.Models
{
    public class AcademicClaim
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LecturerEmail { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [Range(0, 1000)]
        public decimal Hours { get; set; }


        [Range(0, 100000)]
        public decimal Rate { get; set; }


        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;


        public string? FilePath { get; set; }


        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? ProcessedBy { get; set; }
        public DateTime? ProcessedAt { get; set; }


        public decimal Amount => Math.Round(Hours * Rate, 2);
    }
}