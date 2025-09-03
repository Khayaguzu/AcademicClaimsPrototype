using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    // Restrict access so only users with the "Lecturer" role can access this controller
    [AuthorizeRole("Lecturer")]
    public class ClaimsController : Controller
    {
        // Allowed file extensions for uploads
        private static readonly string[] AllowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };

        // Maximum allowed file size (5MB)
        private const long MaxFileBytes = 5 * 1024 * 1024; // 5MB

        // Hosting environment (used to access web root path for saving files)
        private readonly IWebHostEnvironment _env;

        // Constructor to inject hosting environment
        public ClaimsController(IWebHostEnvironment env) => _env = env;

        // Display list of claims for the logged-in lecturer
        public IActionResult Index()
        {
            // Get lecturer email from session
            var email = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail)!;

            // Get all claims belonging to the lecturer, sorted by date (latest first)
            var myClaims = InMemoryStore.Claims
                .Where(c => c.LecturerEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.Date)
                .ToList();

            // Count claims by status and pass to view
            ViewBag.PendingCount = myClaims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.ApprovedCount = myClaims.Count(c => c.Status == ClaimStatus.Approved);
            ViewBag.RejectedCount = myClaims.Count(c => c.Status == ClaimStatus.Rejected);

            return View(myClaims); // Return list of lecturer's claims to view
        }

        // Show claim creation form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handle claim creation form submission
        [HttpPost]
        public async Task<IActionResult> Create(string description, IFormFile file)
        {
            // Validate if file is selected
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a file";
                return View();
            }

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Invalid file type. Allowed: PDF, DOCX, XLSX";
                return View();
            }

            // Validate file size
            if (file.Length > MaxFileBytes)
            {
                ViewBag.Error = "File too large. Maximum size is 5MB";
                return View();
            }

            // Save uploaded file to wwwroot/uploads
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Generate unique file name to avoid conflicts
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Copy file to server storage
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create new claim record
            var email = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail)!;

            var claim = new Claim
            {
                LecturerEmail = email,               // Link claim to logged-in lecturer
                Description = description,           // Description entered by lecturer
                Date = DateTime.Now,                 // Current timestamp
                Hours = 0,                           // Default hours (can be extended in form later)
                Rate = 0,                            // Default rate (can be extended in form later)
                FilePath = $"/uploads/{fileName}",   // Path to uploaded file
                Status = ClaimStatus.Pending         // Default status: Pending
            };

            // Add claim to in-memory data store
            InMemoryStore.Claims.Add(claim);

            // Redirect back to claim list
            return RedirectToAction("Index");
        }
    }
}
