using Demo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BMIT2023.Controllers
{
    public class AuthController : Controller
    {
        private readonly DB db;
        private readonly Helper hp;

        public AuthController(DB db, Helper hp)
        {
            this.db = db;
            this.hp = hp;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            string prefix = vm.UserId[0].ToString().ToUpper();
            string redirectUrl = null;
            string userType = null;

            switch (prefix)
            {
                case "A":
                    if (ValidateAdmin(vm))
                    {
                        redirectUrl = "/admin/dashboard";
                        userType = "Admin";
                    }
                    break;
                case "T":
                    if (ValidateTutor(vm))
                    {
                        redirectUrl = "/tutor/dashboard";
                        userType = "Tutor";
                    }
                    break;
                case "S":
                    if (ValidateStudent(vm))
                    {
                        redirectUrl = "/student/index";
                        userType = "Student";
                    }
                    break;
                default:
                    ModelState.AddModelError("", "Invalid ID prefix");
                    return View(vm);
            }

            if (redirectUrl != null)
            {
                TempData["UserType"] = userType;
                return Redirect(redirectUrl);
            }

            ModelState.AddModelError("", "Invalid ID or Password");
            return View(vm);
        }

        private bool ValidateStudent(LoginVM vm)
        {
            var student = db.Students.FirstOrDefault(s => s.StudentId == vm.UserId);
            if (student != null)
            {
                if (student.VerifyStatus == "Pending")
                {
                    ModelState.AddModelError("", "Your account is pending verification.  Please wait for approval.");
                    return false;
                }

                if (hp.VerifyPassword(student.StudentPassword, vm.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes. NameIdentifier, student.StudentId),
                        new Claim(ClaimTypes.Name, student. StudentEmail),
                        new Claim("FullName", student. StudentName),
                        new Claim(ClaimTypes.Role, "Student")
                    };
                    SignInUser(claims);
                    return true;
                }
            }
            return false;
        }

        private bool ValidateAdmin(LoginVM vm)
        {
            var admin = db.Admins.FirstOrDefault(a => a.AdminId == vm.UserId);
            if (admin != null && hp.VerifyPassword(admin.AdminPassword, vm.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.AdminId),
                    new Claim(ClaimTypes.Name, admin.AdminEmail),
                    new Claim("FullName", admin.AdminName),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                SignInUser(claims);
                return true;
            }
            return false;
        }

        private bool ValidateTutor(LoginVM vm)
        {
            var tutor = db.Tutors.FirstOrDefault(t => t.TutorId == vm.UserId);
            if (tutor != null && hp.VerifyPassword(tutor.TutorPassword, vm.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, tutor.TutorId),
                    new Claim(ClaimTypes.Name, tutor.TutorEmail),
                    new Claim("FullName", tutor.TutorName),
                    new Claim(ClaimTypes.Role, "Tutor")
                };
                SignInUser(claims);
                return true;
            }
            return false;
        }

        private void SignInUser(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public IActionResult Register()
        {
            var vm = new RegisterVM
            {
                UserId = NextId(),
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.UserId = NextId();
                return View(vm);
            }

            try
            {
                var student = new Student
                {
                    StudentId = NextId(),
                    StudentName = vm.FullName?.Trim().ToUpper() ?? "",
                    StudentEmail = vm.Email?.Trim() ?? "",
                    StudentPhone = vm.Phone?.Trim() ?? "",
                    StudentPassword = hp.HashPassword(vm.Password),
                    VerifyStatus = "Pending",
                    StudentGender = vm.Gender
                };

                db.Students.Add(student);
                db.SaveChanges();

                TempData["Success"] = $"Registration successful! Your Student ID is {student.StudentId}.  Please wait for admin approval.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                vm.UserId = NextId();
                return View(vm);
            }
        }

        private string NextId()
        {
            var maxId = db.Students
                .OrderByDescending(s => s.StudentId)
                .FirstOrDefault()?.StudentId ?? "S000";

            int nextNumber;
            if (int.TryParse(maxId[1..], out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
            else
            {
                nextNumber = 1;
            }

            string newId;
            do
            {
                newId = $"S{nextNumber:D3}";
                nextNumber++;
            } while (db.Students.Any(s => s.StudentId == newId));

            return newId;
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}