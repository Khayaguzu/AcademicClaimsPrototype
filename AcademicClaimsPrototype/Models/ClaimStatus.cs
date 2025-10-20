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
        [Range(0.5, 1000, ErrorMessage = "Hours must be between 0.5 and 1000")]
        public double Hours { get; set; }

        [Required(ErrorMessage = "Rate is required")]
        [Range(1, 100000, ErrorMessage = "Rate must be between 1 and 100,000")]
        public double Rate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        // Make DocumentPath optional again to fix the issue
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