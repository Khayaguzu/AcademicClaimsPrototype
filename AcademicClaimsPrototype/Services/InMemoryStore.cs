using AcademicClaimsPrototype.Models;

namespace AcademicClaimsPrototype.Services
{
    public static class InMemoryStore
    {
        public static List<Claim> Claims { get; set; } = new List<Claim>
        {
            new Claim
            {
                Id = "1",
                LecturerEmail = "lecturer1@uni.ac.za",
                Description = "Marking scripts - March 2025",
                Date = DateTime.Now.AddDays(-5),
                Hours = 10,
                Rate = 150,
                Status = ClaimStatus.Pending
            },
            new Claim
            {
                Id = "2",
                LecturerEmail = "lecturer1@uni.ac.za",
                Description = "Invigilating exams - April 2025",
                Date = DateTime.Now.AddDays(-2),
                Hours = 8,
                Rate = 200,
                Status = ClaimStatus.Approved
            },
            new Claim
            {
                Id = "3",
                LecturerEmail = "lecturer2@uni.ac.za",
                Description = "Setting test papers",
                Date = DateTime.Now.AddDays(-1),
                Hours = 5,
                Rate = 180,
                Status = ClaimStatus.Rejected
            }
        };

        public static List<User> Users { get; set; } = new List<User>
        {
            new User { Email = "lecturer1@uni.ac.za", Password = "123", Role = "Lecturer" },
            new User { Email = "lecturer2@uni.ac.za", Password = "123", Role = "Lecturer" },
            new User { Email = "manager@uni.ac.za", Password = "123", Role = "AcademicManager" },
            new User { Email = "coordinator@uni.ac.za", Password = "123", Role = "ProgrammeCoordinator" }
        };
    }
}