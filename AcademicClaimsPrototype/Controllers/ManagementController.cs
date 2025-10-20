using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcademicClaimsPrototype.Controllers
{
    [AuthorizeRole("AcademicManager,ProgrammeCoordinator")]
    public class ManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allClaims = await _context.Claims
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return View(allClaims);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid claim ID.";
                return RedirectToAction("Index");
            }

            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Approved;
                var processedBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
                claim.ProcessedBy = processedBy ?? "Unknown";
                claim.ProcessedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(string id, string reason)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid claim ID.";
                return RedirectToAction("Index");
            }

            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Rejected;
                var processedBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
                claim.ProcessedBy = processedBy ?? "Unknown";
                claim.ProcessedAt = DateTime.UtcNow;
                claim.RejectionReason = string.IsNullOrWhiteSpace(reason)
                    ? "No reason provided"
                    : reason;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult ViewDocument(string documentPath)
        {
            if (string.IsNullOrEmpty(documentPath))
            {
                return NotFound();
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", documentPath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            var contentType = GetContentType(fullPath);
            return File(fileBytes, contentType, Path.GetFileName(fullPath));
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".jpg", "image/jpeg" },
                { ".png", "image/png" }
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}