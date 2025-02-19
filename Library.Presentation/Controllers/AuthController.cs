using Library.Domain.Services;
using Library.Presentation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _authService.RegisterAsync(
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    model.PhoneNumber
                );

                // Redirect to login or home page after successful registration
                return RedirectToAction("Login");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _authService.LoginAsync(model.Email, model.Password);

                List<Claim> claims =
                [
                    new(ClaimTypes.NameIdentifier, $"{user.Id}"),
                    new(ClaimTypes.Name, user.FirstName),
                ];

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        // GET: /Auth/ValidateEmail
        public async Task<IActionResult> ValidateEmail(string email)
        {
            var isValid = await _authService.ValidateEmailAsync(email);
            return Json(new { isValid });
        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}