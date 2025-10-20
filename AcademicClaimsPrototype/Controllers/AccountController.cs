using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Data;
using AcademicClaimsPrototype.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AcademicClaimsPrototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetString(AuthorizeRoleAttribute.SessionEmail, user.Email);
            HttpContext.Session.SetString(AuthorizeRoleAttribute.SessionRole, user.Role);

            if (user.Role == "Lecturer")
                return RedirectToAction("Index", "Claims");
            else
                return RedirectToAction("Index", "Management");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string role)
        {
            // Check if user exists using case-insensitive comparison
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (existingUser != null)
            {
                ViewBag.Error = "User with this email already exists";
                return View();
            }

            var user = new User
            {
                Email = email,
                Password = password,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}