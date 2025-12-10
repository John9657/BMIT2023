using BMIT2023.Data;
using BMIT2023.Models;
using BMIT2023.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BMIT2023.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AuthController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid ID or Password");
                return View(model);
            }

            if (user.VerifyStatus == "Pending")
            {
                ModelState.AddModelError("", "Your account is pending verification. Please wait for approval.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["Success"] = $"Welcome back, {user.Name}!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            try
            {
                string userId = GenerateUserId();
                string? avatarUrl = null;

                if (model.Photo != null)
                {
                    avatarUrl = await SavePhotoAsync(model.Photo, "Users");
                }

                var user = new User
                {
                    UserId = userId,
                    Name = model.Name.Trim(),
                    Email = model.Email.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Phone = model.Phone.Trim(),
                    Gender = model.Gender,
                    AvatarURL = avatarUrl,
                    VerifyStatus = "Pending",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                TempData["Success"] = $"Registration successful! Your User ID is {userId}. Please wait for verification.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Info"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        private string GenerateUserId()
        {
            var lastUser = _db.Users.OrderByDescending(u => u.UserId).FirstOrDefault();
            int nextNumber = 1;

            if (lastUser != null && lastUser.UserId.StartsWith("U"))
            {
                if (int.TryParse(lastUser.UserId.Substring(1), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string newId;
            do
            {
                newId = $"U{nextNumber:D3}";
                nextNumber++;
            } while (_db.Users.Any(u => u.UserId == newId));

            return newId;
        }

        private async Task<string> SavePhotoAsync(IFormFile photo, string folder)
        {
            if (photo == null || photo.Length == 0)
                throw new InvalidOperationException("No photo provided");

            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return $"/uploads/{folder}/{uniqueFileName}";
        }
    }
}