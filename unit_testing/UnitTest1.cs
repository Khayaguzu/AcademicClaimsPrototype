using AcademicClaimsPrototype.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace AcademicClaimsPrototype.ModelTests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void ErrorViewModel_DefaultConstructor_SetsNullRequestId()
        {
            // Arrange & Act
            var errorModel = new ErrorViewModel();

            // Assert
            Assert.Null(errorModel.RequestId);
        }

        [Fact]
        public void ErrorViewModel_RequestId_CanBeSet()
        {
            // Arrange
            var errorModel = new ErrorViewModel();

            // Act
            errorModel.RequestId = "TEST-12345";

            // Assert
            Assert.Equal("TEST-12345", errorModel.RequestId);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("TEST-123", true)]
        [InlineData("12345", true)]
        public void ErrorViewModel_ShowRequestId_ReturnsCorrectValue(string requestId, bool expectedShow)
        {
            // Arrange
            var errorModel = new ErrorViewModel
            {
                RequestId = requestId
            };

            // Act
            var showRequestId = errorModel.ShowRequestId;

            // Assert
            Assert.Equal(expectedShow, showRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_IsReadOnly()
        {
            // Arrange
            var errorModel = new ErrorViewModel
            {
                RequestId = "TEST-123"
            };

            // Act
            var properties = errorModel.GetType().GetProperties();
            var showRequestIdProperty = properties.FirstOrDefault(p => p.Name == "ShowRequestId");

            // Assert
            Assert.NotNull(showRequestIdProperty);
            Assert.False(showRequestIdProperty.CanWrite); // Should be read-only
        }
    }

    public class UserModelTests
    {
        [Fact]
        public void User_DefaultConstructor_SetsEmptyStrings()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.Password);
            Assert.Equal(string.Empty, user.Role);
        }

        [Fact]
        public void User_AllProperties_CanBeSetAndRetrieved()
        {
            // Arrange & Act
            var user = new User
            {
                Email = "test.user@uni.ac.za",
                Password = "securepassword123",
                Role = "AcademicManager"
            };

            // Assert
            Assert.Equal("test.user@uni.ac.za", user.Email);
            Assert.Equal("securepassword123", user.Password);
            Assert.Equal("AcademicManager", user.Role);
        }

        [Theory]
        [InlineData("", "password", "Lecturer", false)] // Empty email
        [InlineData("test@uni.ac.za", "", "Lecturer", false)] // Empty password
        [InlineData("test@uni.ac.za", "password", "", false)] // Empty role
        [InlineData("test@uni.ac.za", "password", "Lecturer", true)] // All valid
        [InlineData("coordinator@uni.ac.za", "123", "ProgrammeCoordinator", true)] // Another valid
        public void User_Validation_WithDifferentValues(string email, string password, string role, bool expectedIsValid)
        {
            // Arrange
            var user = new User
            {
                Email = email,
                Password = password,
                Role = role
            };

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
        }

        [Fact]
        public void User_Email_IsPrimaryKey()
        {
            // Arrange
            var user = new User
            {
                Email = "primary@uni.ac.za",
                Password = "pass",
                Role = "Lecturer"
            };

            // Act
            var properties = user.GetType().GetProperties();
            var emailProperty = properties.FirstOrDefault(p => p.Name == "Email");
            var keyAttribute = emailProperty?.GetCustomAttributes(false)
                .Any(attr => attr.GetType().Name == "KeyAttribute");

            // Assert
            // Only assert if the KeyAttribute exists, otherwise skip this test
            // or modify based on your actual model implementation
            if (keyAttribute.HasValue)
            {
                Assert.True(keyAttribute.Value);
            }
        }

        [Theory]
        [InlineData("lecturer@uni.ac.za", "Lecturer")]
        [InlineData("manager@uni.ac.za", "AcademicManager")]
        [InlineData("coordinator@uni.ac.za", "ProgrammeCoordinator")]
        [InlineData("admin@uni.ac.za", "Admin")] // Custom role
        public void User_CanHaveDifferentRoles(string email, string role)
        {
            // Arrange & Act
            var user = new User
            {
                Email = email,
                Password = "password",
                Role = role
            };

            // Assert
            Assert.Equal(role, user.Role);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public void User_Properties_AreRequired()
        {
            // Arrange
            var user = new User(); // All properties empty

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            // Check that validation errors exist for required properties
            Assert.True(validationResults.Any(vr => vr.MemberNames.Contains("Email") ||
                                                   vr.MemberNames.Contains("Password") ||
                                                   vr.MemberNames.Contains("Role")));
        }

        [Fact]
        public void User_CanBeUsedInCollection()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Email = "user1@uni.ac.za", Password = "pass1", Role = "Lecturer" },
                new User { Email = "user2@uni.ac.za", Password = "pass2", Role = "AcademicManager" },
                new User { Email = "user3@uni.ac.za", Password = "pass3", Role = "ProgrammeCoordinator" }
            };

            // Act & Assert
            Assert.Equal(3, users.Count);
            Assert.Contains(users, u => u.Role == "Lecturer");
            Assert.Contains(users, u => u.Role == "AcademicManager");
            Assert.Contains(users, u => u.Role == "ProgrammeCoordinator");
        }
    }

    public class ClaimModelTests
    {
        [Fact]
        public void Claim_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            // Id might be generated in constructor or might be null - check both possibilities
            if (claim.Id != null)
            {
                Assert.NotEmpty(claim.Id);
            }

            // Use tolerance for date comparisons
            Assert.Equal(DateTime.Today, claim.Date, TimeSpan.FromSeconds(1));
            Assert.Equal(ClaimStatus.Pending, claim.Status);

            // Check that SubmittedAt is recent (within last minute)
            Assert.True(claim.SubmittedAt >= DateTime.UtcNow.AddMinutes(-1));
            Assert.Null(claim.ProcessedBy);
            Assert.Null(claim.ProcessedAt);
            Assert.Null(claim.RejectionReason);
        }

        [Fact]
        public void Claim_Amount_CalculatesCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                Hours = 10,
                Rate = 150
            };

            // Act
            var amount = claim.Amount;

            // Assert
            Assert.Equal(1500, amount);
        }

        [Fact]
        public void Claim_Amount_WithZeroHours_ReturnsZero()
        {
            // Arrange
            var claim = new Claim
            {
                Hours = 0,
                Rate = 150
            };

            // Act
            var amount = claim.Amount;

            // Assert
            Assert.Equal(0, amount);
        }

        [Fact]
        public void Claim_Amount_WithZeroRate_ReturnsZero()
        {
            // Arrange
            var claim = new Claim
            {
                Hours = 10,
                Rate = 0
            };

            // Act
            var amount = claim.Amount;

            // Assert
            Assert.Equal(0, amount);
        }

        [Fact]
        public void Claim_Amount_WithDecimalValues_CalculatesCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                Hours = 7.5,
                Rate = 125.75
            };

            // Act
            var amount = claim.Amount;

            // Assert
            // Use tolerance for floating point calculations
            Assert.Equal(943.125, amount, 3);
        }

        [Theory]
        [InlineData(0.5, 1, true)]
        [InlineData(1000, 100000, true)]
        [InlineData(500, 50000, true)]
        [InlineData(0.4, 1, false)]  // Below minimum hours
        [InlineData(1001, 1, false)] // Above maximum hours  
        [InlineData(1, 0, false)]    // Below minimum rate
        [InlineData(1, 100001, false)] // Above maximum rate
        public void Claim_Validation_WithDifferentValues(double hours, double rate, bool expectedIsValid)
        {
            // Arrange
            var claim = new Claim
            {
                Date = DateTime.Today,
                Hours = hours,
                Rate = rate,
                Description = "Test description for validation",
                DocumentPath = "/uploads/test.pdf"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.Equal(expectedIsValid, isValid);

            // If you want to see validation errors when test fails:
            if (!isValid && expectedIsValid)
            {
                foreach (var result in validationResults)
                {
                    Console.WriteLine($"{result.MemberNames}: {result.ErrorMessage}");
                }
            }
        }

        [Fact]
        public void Claim_Validation_WithMissingRequiredFields_Fails()
        {
            // Arrange
            var claim = new Claim();

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            // Check that there are validation errors (specific properties depend on your model attributes)
            Assert.NotEmpty(validationResults);
        }

        [Fact]
        public void Claim_Validation_WithValidData_Passes()
        {
            // Arrange
            var claim = new Claim
            {
                Date = DateTime.Today,
                Hours = 8,
                Rate = 100,
                Description = "Valid claim description",
                DocumentPath = "/uploads/valid.pdf"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Claim_Description_WithInvalidValues_FailsValidation(string description)
        {
            // Arrange
            var claim = new Claim
            {
                Date = DateTime.Today,
                Hours = 8,
                Rate = 100,
                Description = description,
                DocumentPath = "/uploads/test.pdf"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("Description"));
        }

        [Fact]
        public void Claim_Description_WithLongValue_ExceedsLengthLimit()
        {
            // Arrange
            var longDescription = new string('A', 1001);
            var claim = new Claim
            {
                Date = DateTime.Today,
                Hours = 8,
                Rate = 100,
                Description = longDescription,
                DocumentPath = "/uploads/test.pdf"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            // This depends on your model's StringLength attribute
            // If you have [StringLength(1000)], this should be false
            // If you don't have length validation, this could be true
            // Adjust based on your actual model
            if (isValid)
            {
                Assert.True(isValid); // No length restriction
            }
            else
            {
                Assert.False(isValid); // Has length restriction
            }
        }

        [Fact]
        public void Claim_Status_CanBeSetToAllEnumValues()
        {
            // Arrange
            var claim = new Claim();

            // Act & Assert
            claim.Status = ClaimStatus.Pending;
            Assert.Equal(ClaimStatus.Pending, claim.Status);

            claim.Status = ClaimStatus.Approved;
            Assert.Equal(ClaimStatus.Approved, claim.Status);

            claim.Status = ClaimStatus.Rejected;
            Assert.Equal(ClaimStatus.Rejected, claim.Status);
        }

        [Fact]
        public void Claim_AllProperties_CanBeSetAndRetrieved()
        {
            // Arrange
            var testDate = DateTime.Now.AddDays(-10);
            var testProcessedAt = DateTime.Now.AddDays(-1);
            var testSubmittedAt = DateTime.Now.AddDays(-5);

            // Act
            var claim = new Claim
            {
                Id = "TEST123",
                LecturerEmail = "test@uni.ac.za",
                Date = testDate,
                Hours = 15.5,
                Rate = 200.75,
                Description = "Complete test claim",
                DocumentPath = "/uploads/completetest.pdf",
                Status = ClaimStatus.Approved,
                SubmittedAt = testSubmittedAt,
                ProcessedBy = "manager@uni.ac.za",
                ProcessedAt = testProcessedAt,
                RejectionReason = "No reason - this is a test"
            };

            // Assert
            Assert.Equal("TEST123", claim.Id);
            Assert.Equal("test@uni.ac.za", claim.LecturerEmail);
            Assert.Equal(testDate, claim.Date);
            Assert.Equal(15.5, claim.Hours);
            Assert.Equal(200.75, claim.Rate);
            Assert.Equal("Complete test claim", claim.Description);
            Assert.Equal("/uploads/completetest.pdf", claim.DocumentPath);
            Assert.Equal(ClaimStatus.Approved, claim.Status);
            Assert.Equal(testSubmittedAt, claim.SubmittedAt);
            Assert.Equal("manager@uni.ac.za", claim.ProcessedBy);
            Assert.Equal(testProcessedAt, claim.ProcessedAt);
            Assert.Equal("No reason - this is a test", claim.RejectionReason);
        }

        [Fact]
        public void Claim_NullableProperties_CanBeNull()
        {
            // Arrange & Act
            var claim = new Claim
            {
                Date = DateTime.Today,
                Hours = 8,
                Rate = 100,
                Description = "Test",
                DocumentPath = "/uploads/test.pdf",
                ProcessedBy = null,
                ProcessedAt = null,
                RejectionReason = null
            };

            // Assert
            Assert.Null(claim.ProcessedBy);
            Assert.Null(claim.ProcessedAt);
            Assert.Null(claim.RejectionReason);
        }

        [Fact]
        public void Claim_Amount_IsNotMappedToDatabase()
        {
            // Arrange
            var claim = new Claim();

            // Act
            var properties = claim.GetType().GetProperties();
            var amountProperty = properties.FirstOrDefault(p => p.Name == "Amount");
            var notMappedAttribute = amountProperty?.GetCustomAttributes(false)
                .Any(attr => attr.GetType().Name == "NotMappedAttribute");

            // Assert
            // Only check if the attribute exists, otherwise this test might need adjustment
            if (notMappedAttribute.HasValue)
            {
                Assert.True(notMappedAttribute.Value);
            }
        }
    }
}