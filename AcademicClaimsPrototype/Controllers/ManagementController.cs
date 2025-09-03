using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    [AuthorizeRole("AcademicManager,ProgrammeCoordinator")]
    public class ManagementController : Controller
    {
        public IActionResult Index()
        {
            var allClaims = InMemoryStore.Claims
                .OrderByDescending(c => c.Date)
                .ToList();

            return View(allClaims);
        }

        [HttpPost]
        public IActionResult Approve(string id)
        {
            var claim = InMemoryStore.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Approved;
                claim.ProcessedBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
                claim.ProcessedAt = DateTime.UtcNow;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(string id)
        {
            var claim = InMemoryStore.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                claim.Status = ClaimStatus.Rejected;
                claim.ProcessedBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail);
                claim.ProcessedAt = DateTime.UtcNow;
            }
            return RedirectToAction("Index");
        }
    }
}