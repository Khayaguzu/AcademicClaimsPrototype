using System.ComponentModel.DataAnnotations;

namespace AcademicClaimsPrototype.Models
{
    public class Claim
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LecturerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Hours are required")]
        [Range(0, 1000, ErrorMessage = "Hours must be between 0 and 1000")]
        public double Hours { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        [Range(0, 100000, ErrorMessage = "Rate must be between 0 and 100,000")]
        public double Rate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        public string? DocumentPath { get; set; }
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? ProcessedBy { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? RejectionReason { get; set; }

        public double Amount => Hours * Rate;
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }
}