using Microsoft.EntityFrameworkCore;
using AcademicClaimsPrototype.Models;
using System;

namespace AcademicClaimsPrototype.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.Email);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasConversion(
                    v => v.ToLower(),
                    v => v
                );

            // Configure Claim entity
            modelBuilder.Entity<Claim>()
                .Property(c => c.LecturerEmail)
                .IsRequired();

            modelBuilder.Entity<Claim>()
                .Property(c => c.DocumentPath)
                .IsRequired();

            // Seed initial users (including HR)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Email = "lecturer1@uni.ac.za",
                    Password = "123",
                    Role = "Lecturer",
                    FullName = "Lecturer One",
                    PhoneNumber = "0810000001",
                    Department = "CS"
                },
                new User
                {
                    Email = "lecturer2@uni.ac.za",
                    Password = "123",
                    Role = "Lecturer",
                    FullName = "Lecturer Two",
                    PhoneNumber = "0810000002",
                    Department = "IT"
                },
                new User
                {
                    Email = "manager@uni.ac.za",
                    Password = "123",
                    Role = "AcademicManager"
                },
                new User
                {
                    Email = "coordinator@uni.ac.za",
                    Password = "123",
                    Role = "ProgrammeCoordinator"
                },
                new User
                {
                    Email = "hr@uni.ac.za",
                    Password = "123",
                    Role = "HR"
                }
            );

            // Seed initial claims - UPDATED with payment info
            modelBuilder.Entity<Claim>().HasData(
                new Claim
                {
                    Id = "1",
                    LecturerEmail = "lecturer1@uni.ac.za",
                    Description = "Marking scripts - March 2025",
                    Date = DateTime.Now.AddDays(-5),
                    Hours = 10,
                    Rate = 150,
                    Status = ClaimStatus.Pending,
                    DocumentPath = "/uploads/marking-scripts.pdf",
                    SubmittedAt = DateTime.UtcNow.AddDays(-5)
                },
                new Claim
                {
                    Id = "2",
                    LecturerEmail = "lecturer1@uni.ac.za",
                    Description = "Invigilating exams - April 2025",
                    Date = DateTime.Now.AddDays(-2),
                    Hours = 8,
                    Rate = 200,
                    Status = ClaimStatus.Approved,
                    DocumentPath = "/uploads/invigilation.pdf",
                    ProcessedBy = "manager@uni.ac.za",
                    ProcessedAt = DateTime.Now.AddDays(-1),
                    SubmittedAt = DateTime.UtcNow.AddDays(-2),
                    IsPaid = true,
                    PaidDate = DateTime.Now,
                    PaidBy = "hr@uni.ac.za",
                    PaymentReference = "PAY-001"
                },
                new Claim
                {
                    Id = "3",
                    LecturerEmail = "lecturer2@uni.ac.za",
                    Description = "Setting test papers",
                    Date = DateTime.Now.AddDays(-1),
                    Hours = 5,
                    Rate = 180,
                    Status = ClaimStatus.Rejected,
                    DocumentPath = "/uploads/test-paper.pdf",
                    ProcessedBy = "coordinator@uni.ac.za",
                    ProcessedAt = DateTime.Now,
                    RejectionReason = "Duplicate task submission",
                    SubmittedAt = DateTime.UtcNow.AddDays(-1)
                }
            );
        }
    }
}