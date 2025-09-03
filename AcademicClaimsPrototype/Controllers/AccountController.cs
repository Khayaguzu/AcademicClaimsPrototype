using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = InMemoryStore.Users
                .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                     && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Store user info in session using the same keys as AuthorizeRoleAttribute
            HttpContext.Session.SetString(AcademicClaimsPrototype.Filters.AuthorizeRoleAttribute.SessionEmail, user.Email);
            HttpContext.Session.SetString(AcademicClaimsPrototype.Filters.AuthorizeRoleAttribute.SessionRole, user.Role);

            // Redirect based on role
            if (user.Role == "Lecturer")
                return RedirectToAction("Index", "Claims");
            else
                return RedirectToAction("Index", "Management");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string email, string password, string role)
        {
            if (InMemoryStore.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "User with this email already exists";
                return View();
            }

            // Add new user to in-memory store
            InMemoryStore.Users.Add(new User
            {
                Email = email,
                Password = password,
                Role = role
            });

            ViewBag.Message = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
