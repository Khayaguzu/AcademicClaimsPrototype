using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    public class DashboardController : Controller
    {
        // Entry point for the Dashboard
        public IActionResult Index()
        {
            // Get the user's role from session
            var role = HttpContext.Session.GetString(AcademicClaimsPrototype.Filters.AuthorizeRoleAttribute.SessionRole);

            // If no role is found in session, redirect to the Login page
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");

            // If the user is a Lecturer, redirect them to their Claims page
            if (role.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Claims");

            // Otherwise, redirect them to the Approvals page (e.g., for managers/admins)
            return RedirectToAction("Index", "Approvals");
        }
    }
}
