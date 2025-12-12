using Microsoft.AspNetCore.Mvc;
using BMIT2023.Models;
using BMIT2023.Data;
using System.Linq;

namespace BMIT2023.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login - Checks Student, Teacher, and Admin tables
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var id = model.UserId.ToUpper();
                object user = null;
                string role = "";

                // 1. Check Student Table
                var student = _context.Students.FirstOrDefault(u => u.StudentId == id && u.Password == model.Password);
                if (student != null)
                {
                    user = student;
                    role = "Student";
                }

                // 2. Check Teacher Table
                if (user == null)
                {
                    var teacher = _context.Teachers.FirstOrDefault(u => u.TeacherId == id && u.Password == model.Password);
                    if (teacher != null)
                    {
                        user = teacher;
                        role = "Teacher";
                    }
                }

                // 3. Check Admin Table
                if (user == null)
                {
                    var admin = _context.Admins.FirstOrDefault(u => u.AdminId == id && u.Password == model.Password);
                    if (admin != null)
                    {
                        user = admin;
                        role = "Admin";
                    }
                }

                if (user != null)
                {
                    // Success! You should now set a session/cookie here to maintain login state.
                    TempData["UserRole"] = role; // Storing role temporarily for demo
                    TempData["UserId"] = id;
                    // Redirect to Billing Dashboard instead of Home
                    return RedirectToAction("Dashboard", "Billing");
                }

                ModelState.AddModelError("", "Invalid User ID or Password");
            }
            return View(model);
        }

        // GET: Register (For Students Only)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register (For Students Only)
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Generate the Student ID (SXXX)
                string prefix = "S";

                // Finds the count of all existing students
                int nextNumber = _context.Students.Count() + 1;

                string generatedId = $"{prefix}{nextNumber.ToString("D3")}"; // Formats 1 to "001"

                // 2. Create the Student
                var newStudent = new Student
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    StudentId = generatedId, // Save the generated ID
                    Password = model.Password
                };

                _context.Students.Add(newStudent);
                _context.SaveChanges();

                // 3. Redirect to success page to display their ID
                TempData["GeneratedId"] = generatedId;
                return RedirectToAction("RegistrationSuccess");
            }
            return View(model);
        }

        // GET: Registration Success
        [HttpGet]
        public IActionResult RegistrationSuccess()
        {
            if (TempData["GeneratedId"] == null) return RedirectToAction("Login");

            ViewBag.UserId = TempData["GeneratedId"];
            return View();
        }
    }
}