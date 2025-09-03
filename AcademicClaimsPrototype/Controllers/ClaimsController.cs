using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    [AuthorizeRole("Lecturer")]
    public class ClaimsController : Controller
    {
        private static readonly string[] AllowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
        private const long MaxFileBytes = 5 * 1024 * 1024; // 5MB
        private readonly IWebHostEnvironment _env;

        public ClaimsController(IWebHostEnvironment env) => _env = env;

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail)!;
            var myClaims = InMemoryStore.Claims
                .Where(c => c.LecturerEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(c => c.Date)
                .ToList();

            ViewBag.PendingCount = myClaims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.ApprovedCount = myClaims.Count(c => c.Status == ClaimStatus.Approved);
            ViewBag.RejectedCount = myClaims.Count(c => c.Status == ClaimStatus.Rejected);

            return View(myClaims);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string description, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a file";
                return View();
            }

            // Validate file
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ViewBag.Error = "Invalid file type. Allowed: PDF, DOCX, XLSX";
                return View();
            }

            if (file.Length > MaxFileBytes)
            {
                ViewBag.Error = "File too large. Maximum size is 5MB";
                return View();
            }

            // Save file
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create claim
            var email = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail)!;

            var claim = new Claim
            {
                LecturerEmail = email,
                Description = description,
                Date = DateTime.Now,
                Hours = 0, // You might want to add form fields for these
                Rate = 0,  // You might want to add form fields for these
                FilePath = $"/uploads/{fileName}",
                Status = ClaimStatus.Pending
            };

            InMemoryStore.Claims.Add(claim);

            return RedirectToAction("Index");
        }
    }
}