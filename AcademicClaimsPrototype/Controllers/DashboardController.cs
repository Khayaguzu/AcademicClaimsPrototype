using AcademicClaimsPrototype.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionRole);
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");

            if (role.Equals("Lecturer", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Claims");

            return RedirectToAction("Index", "Management");
        }
    }
}