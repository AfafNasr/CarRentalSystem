using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace CarRentalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AccountController(CarRentalDbContext context, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest();
            }

            var viewModel =
                new ResetPasswordViewModel
                {
                    Token = token
                };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
    ForgotPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var normalizedEmail =
                viewModel.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == normalizedEmail);

            if (user != null)
            {
                var oldTokens =
                    await _context.PasswordResetTokens
                        .Where(t =>
                            t.UserId == user.Id &&
                            !t.IsUsed)
                        .ToListAsync();

                foreach (var oldToken in oldTokens)
                {
                    oldToken.IsUsed = true;
                }

                var tokenBytes =
                    RandomNumberGenerator.GetBytes(32);

                var token =
                    WebEncoders.Base64UrlEncode(tokenBytes);

                var tokenHash =
                    Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes(token)));

                var passwordResetToken =
                    new PasswordResetToken
                    {
                        UserId = user.Id,

                        TokenHash = tokenHash,

                        ExpiresAt =
                            DateTime.UtcNow.AddMinutes(30)
                    };

                _context.PasswordResetTokens
                    .Add(passwordResetToken);

                await _context.SaveChangesAsync();

                var resetLink =
                    Url.Action(
                        nameof(ResetPassword),
                        "Account",
                        new { token },
                        Request.Scheme);

                if (!string.IsNullOrWhiteSpace(resetLink))
                {
                    await _emailService
                        .SendPasswordResetEmailAsync(
                            user.Email,
                            resetLink);
                }
            }

            TempData["PasswordResetMessage"] =
                "If an account exists with this email, " +
                "a password reset link has been sent.";

            return RedirectToAction(nameof(ForgotPassword));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
    ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(
                            viewModel.Token)));

            var resetToken =
                await _context.PasswordResetTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t =>
                        t.TokenHash == tokenHash &&
                        !t.IsUsed);

            if (resetToken == null ||
                resetToken.ExpiresAt <= DateTime.UtcNow)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This password reset link is invalid or has expired.");

                return View(viewModel);
            }

            var passwordHasher =
                new PasswordHasher<User>();

            resetToken.User.PasswordHash =
                passwordHasher.HashPassword(
                    resetToken.User,
                    viewModel.NewPassword);

            resetToken.IsUsed = true;

            await _context.SaveChangesAsync();

            TempData["LoginSuccess"] =
                "Your password has been reset successfully. " +
                "You can now sign in with your new password.";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == normalizedEmail);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = normalizedEmail,
                PhoneNumber = model.PhoneNumber.Trim(),
                DateOfBirth = model.DateOfBirth,
                AddressLine1 = model.AddressLine1.Trim(),
                AddressLine2 = model.AddressLine2?.Trim(),
                City = model.City.Trim(),
                Country = model.Country.Trim(),
                DriversLicenseNumber = model.DriversLicenseNumber.Trim(),
                Role = UserRole.Customer
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var normalizedEmail = model.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    model.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.Id.ToString()),

        new Claim(
            ClaimTypes.Name,
            $"{user.FirstName} {user.LastName}"),

        new Claim(
            ClaimTypes.Email,
            user.Email),

        new Claim(
            ClaimTypes.Role,
            user.Role.ToString())
    };

            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties =
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

            await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity),
    authProperties);

            if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Cars");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                City = user.City,
                Country = user.Country,
                DriversLicenseNumber = user.DriversLicenseNumber
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var normalizedEmail =
                model.Email.Trim().ToLower();

            var emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Email == normalizedEmail &&
                    u.Id != userId);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();
            user.Email = normalizedEmail;
            user.PhoneNumber = model.PhoneNumber.Trim();
            user.DateOfBirth = model.DateOfBirth;
            user.AddressLine1 = model.AddressLine1.Trim();
            user.AddressLine2 = model.AddressLine2?.Trim();
            user.City = model.City.Trim();
            user.Country = model.Country.Trim();
            user.DriversLicenseNumber =
                model.DriversLicenseNumber.Trim();

            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] =
                "Your profile has been updated successfully.";

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    model.CurrentPassword);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    nameof(model.CurrentPassword),
                    "Current password is incorrect.");

                return View(model);
            }

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    model.NewPassword);

            await _context.SaveChangesAsync();

            TempData["PasswordSuccess"] =
                "Your password has been changed successfully.";

            return RedirectToAction(nameof(ChangePassword));
        }
    }
}