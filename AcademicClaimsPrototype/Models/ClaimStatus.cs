// In Models/Claim.cs
namespace AcademicClaimsPrototype.Models
{
    public class Claim
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LecturerEmail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public decimal Rate { get; set; }
        public string? FilePath { get; set; }
        public ClaimStatus Status { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? ProcessedBy { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public decimal Amount => Math.Round(Hours * Rate, 2);
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected
    }
}