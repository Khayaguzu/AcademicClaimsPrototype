using AcademicClaimsPrototype.Filters;
using Microsoft.AspNetCore.Mvc;
using System;

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
            else if (role.Equals("HR", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "HR");

            return RedirectToAction("Index", "Management");
        }
    }
}