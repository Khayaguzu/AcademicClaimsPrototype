using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AcademicClaimsPrototype.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LecturerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "Date", "Description", "DocumentPath", "Hours", "LecturerEmail", "ProcessedAt", "ProcessedBy", "Rate", "RejectionReason", "Status", "SubmittedAt" },
                values: new object[,]
                {
                    { "1", new DateTime(2025, 11, 9, 2, 44, 37, 260, DateTimeKind.Local).AddTicks(9440), "Marking scripts - March 2025", "/uploads/marking-scripts.pdf", 10.0, "lecturer1@uni.ac.za", null, null, 150.0, null, 0, new DateTime(2025, 11, 9, 0, 44, 37, 260, DateTimeKind.Utc).AddTicks(9452) },
                    { "2", new DateTime(2025, 11, 12, 2, 44, 37, 260, DateTimeKind.Local).AddTicks(9474), "Invigilating exams - April 2025", "/uploads/invigilation.pdf", 8.0, "lecturer1@uni.ac.za", new DateTime(2025, 11, 13, 2, 44, 37, 260, DateTimeKind.Local).AddTicks(9478), "manager@uni.ac.za", 200.0, null, 1, new DateTime(2025, 11, 12, 0, 44, 37, 260, DateTimeKind.Utc).AddTicks(9487) },
                    { "3", new DateTime(2025, 11, 13, 2, 44, 37, 260, DateTimeKind.Local).AddTicks(9496), "Setting test papers", "/uploads/test-paper.pdf", 5.0, "lecturer2@uni.ac.za", new DateTime(2025, 11, 14, 2, 44, 37, 260, DateTimeKind.Local).AddTicks(9503), "coordinator@uni.ac.za", 180.0, "Duplicate task submission", 2, new DateTime(2025, 11, 13, 0, 44, 37, 260, DateTimeKind.Utc).AddTicks(9504) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "Department", "FullName", "Password", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { "coordinator@uni.ac.za", null, null, "123", null, "ProgrammeCoordinator" },
                    { "hr@uni.ac.za", null, null, "123", null, "HR" },
                    { "lecturer1@uni.ac.za", "CS", "Lecturer One", "123", "0810000001", "Lecturer" },
                    { "lecturer2@uni.ac.za", "IT", "Lecturer Two", "123", "0810000002", "Lecturer" },
                    { "manager@uni.ac.za", null, null, "123", null, "AcademicManager" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
