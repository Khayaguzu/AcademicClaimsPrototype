using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Data;
using AcademicClaimsPrototype.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Login() => View();

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
            else if (user.Role == "HR")
                return RedirectToAction("Index", "HR");
            else
                return RedirectToAction("Index", "Management");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string role)
        {
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

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Temporary method to create HR user if needed
        [HttpGet]
        public async Task<IActionResult> CreateHRUser()
        {
            var existingHR = await _context.Users.FirstOrDefaultAsync(u => u.Email == "hr@uni.ac.za");
            if (existingHR == null)
            {
                var hrUser = new User
                {
                    Email = "hr@uni.ac.za",
                    Password = "123",
                    Role = "HR"
                };
                _context.Users.Add(hrUser);
                await _context.SaveChangesAsync();
                return Content("HR user created successfully. Email: hr@uni.ac.za, Password: 123");
            }
            return Content("HR user already exists. Email: hr@uni.ac.za, Password: 123");
        }
    }
}