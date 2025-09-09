using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Services;
using AcademicClaimsPrototype.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AcademicClaimsPrototype.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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
        public IActionResult Register(string email, string password, string role)
        {
            if (InMemoryStore.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "User with this email already exists";
                return View();
            }

            InMemoryStore.Users.Add(new User
            {
                Email = email,
                Password = password,
                Role = role
            });

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