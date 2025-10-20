using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AcademicClaimsPrototype.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var userClaims = await _context.Claims
                .Where(c => c.LecturerEmail == userEmail)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            ViewBag.PendingCount = userClaims.Count(c => c.Status == ClaimStatus.Pending);
            ViewBag.ApprovedCount = userClaims.Count(c => c.Status == ClaimStatus.Approved);
            ViewBag.RejectedCount = userClaims.Count(c => c.Status == ClaimStatus.Rejected);

            return View(userClaims);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var claim = new Claim
            {
                Date = DateTime.Today
            };
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, IFormFile file)
        {
            // Get user email from session first
            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Set the LecturerEmail before any validation
            claim.LecturerEmail = userEmail;

            // Check if file is provided
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Document is required");
                return View(claim);
            }

            // Now validate the model (after setting LecturerEmail)
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            try
            {
                claim.SubmittedAt = DateTime.UtcNow;
                claim.Status = ClaimStatus.Pending;

                // File validation
                if (file.Length > 10 * 1024 * 1024) // 10MB limit
                {
                    ModelState.AddModelError("file", "File size must be less than 10MB");
                    return View(claim);
                }

                // Validate file extension
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("file", "Only PDF, Word, JPG, and PNG files are allowed");
                    return View(claim);
                }

                // Ensure upload directory exists
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Save file with unique name
                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Set the document path AFTER successful file upload
                claim.DocumentPath = "/uploads/" + fileName;
                NotifyManagersAboutDocument(claim);

                // Add claim to database
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                Console.WriteLine($"Error creating claim: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                TempData["Error"] = "An error occurred while submitting the claim. Please try again.";
                return View(claim);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id && c.LecturerEmail == userEmail);

            if (claim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction("Index");
            }

            if (claim.Status != ClaimStatus.Pending)
            {
                TempData["Error"] = "You can only edit pending claims.";
                return RedirectToAction("Index");
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Claim updatedClaim, IFormFile file)
        {
            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // Set the LecturerEmail before validation
            updatedClaim.LecturerEmail = userEmail;

            if (!ModelState.IsValid)
            {
                return View(updatedClaim);
            }

            var existingClaim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == updatedClaim.Id && c.LecturerEmail == userEmail);

            if (existingClaim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction("Index");
            }

            if (existingClaim.Status != ClaimStatus.Pending)
            {
                TempData["Error"] = "You can only edit pending claims.";
                return RedirectToAction("Index");
            }

            // Update claim details
            existingClaim.Date = updatedClaim.Date;
            existingClaim.Hours = updatedClaim.Hours;
            existingClaim.Rate = updatedClaim.Rate;
            existingClaim.Description = updatedClaim.Description ?? string.Empty;

            // Handle file upload - required for editing too
            if (file != null && file.Length > 0)
            {
                // File validation
                if (file.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("file", "File size must be less than 10MB");
                    return View(updatedClaim);
                }

                // Validate file extension
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("file", "Only PDF, Word, JPG, and PNG files are allowed");
                    return View(updatedClaim);
                }

                // Remove old file if exists
                if (!string.IsNullOrEmpty(existingClaim.DocumentPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingClaim.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Save new file
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                existingClaim.DocumentPath = "/uploads/" + fileName;
                NotifyManagersAboutDocument(existingClaim, true);
            }
            else
            {
                // If no new file is uploaded, keep the existing document path
                existingClaim.DocumentPath = existingClaim.DocumentPath;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Claim updated successfully!";
            return RedirectToAction("Index");
        }

        private void NotifyManagersAboutDocument(Claim claim, bool isUpdate = false)
        {
            var action = isUpdate ? "updated" : "submitted";
            Console.WriteLine($"Document {action} by {claim.LecturerEmail}");
            Console.WriteLine($"Notifying Programme Coordinator and Manager about claim: {claim.Description}");
            Console.WriteLine($"Document path: {claim.DocumentPath}");
        }
    }
}