using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademicClaimsPrototype.Controllers
{
    public class AccountController : Controller
    {
        // Displays the Login page (GET request)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handles Login form submission (POST request)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Check if user exists in the in-memory user store and validate credentials
            var user = InMemoryStore.Users
                .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                     && u.Password == password);

            // If no user matches, return login view with error message
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Store user information in session for authentication
            HttpContext.Session.SetString(AcademicClaimsPrototype.Filters.AuthorizeRoleAttribute.SessionEmail, user.Email);
            HttpContext.Session.SetString(AcademicClaimsPrototype.Filters.AuthorizeRoleAttribute.SessionRole, user.Role);

            // Redirect users to different controllers depending on their role
            if (user.Role == "Lecturer")
                return RedirectToAction("Index", "Claims");
            else
                return RedirectToAction("Index", "Management");
        }

        // Displays the Register page (GET request)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handles Register form submission (POST request)
        [HttpPost]
        public IActionResult Register(string email, string password, string role)
        {
            // Check if a user with the same email already exists
            if (InMemoryStore.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "User with this email already exists";
                return View();
            }

            // Add new user to the in-memory user store
            InMemoryStore.Users.Add(new User
            {
                Email = email,
                Password = password,
                Role = role
            });

            // Show success message and redirect to login page
            ViewBag.Message = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // Handles Logout (clears session and redirects to Login)
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Login"); // Redirect back to login page
        }
    }
}
