using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AcademicClaimsPrototype.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var userClaims = InMemoryStore.Claims
                .Where(c => c.LecturerEmail == userEmail)
                .OrderByDescending(c => c.Date)
                .ToList();

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
        public async Task<IActionResult> Create(Claim claim, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(claim);
            }

            try
            {
                var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Login", "Account");
                }

                claim.LecturerEmail = userEmail;
                claim.SubmittedAt = DateTime.UtcNow;
                claim.Status = ClaimStatus.Pending;

                if (file != null && file.Length > 0)
                {
                    if (file.Length > 10 * 1024 * 1024)
                    {
                        ModelState.AddModelError("file", "File size must be less than 10MB");
                        return View(claim);
                    }

                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    claim.DocumentPath = "/uploads/" + fileName;
                    NotifyManagersAboutDocument(claim);
                }

                InMemoryStore.Claims.Add(claim);
                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating claim: {ex.Message}");
                TempData["Error"] = "An error occurred while submitting the claim. Please try again.";
                return View(claim);
            }
        }

        [HttpGet]
        public IActionResult Edit(string id)
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

            var claim = InMemoryStore.Claims.FirstOrDefault(c => c.Id == id && c.LecturerEmail == userEmail);

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
        public async Task<IActionResult> Edit(Claim updatedClaim, IFormFile? file)
        {
            if (updatedClaim == null || string.IsNullOrEmpty(updatedClaim.Id))
            {
                TempData["Error"] = "Invalid claim data.";
                return RedirectToAction("Index");
            }

            var userEmail = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var existingClaim = InMemoryStore.Claims.FirstOrDefault(c => c.Id == updatedClaim.Id && c.LecturerEmail == userEmail);

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

            existingClaim.Date = updatedClaim.Date;
            existingClaim.Hours = updatedClaim.Hours;
            existingClaim.Rate = updatedClaim.Rate;
            existingClaim.Description = updatedClaim.Description ?? string.Empty;

            if (file != null && file.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingClaim.DocumentPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingClaim.DocumentPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                existingClaim.DocumentPath = "/uploads/" + fileName;
                NotifyManagersAboutDocument(existingClaim, true);
            }

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