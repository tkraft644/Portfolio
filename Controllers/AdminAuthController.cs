using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Models;
using Portfolio.Models.Settings;

namespace Portfolio.Controllers;

[Route("admin")]
public class AdminAuthController : Controller
{
    private readonly AdminSettings _settings;

    public AdminAuthController(IOptions<AdminSettings> settings)
    {
        _settings = settings.Value;
    }

    private bool IsEnabled => _settings.Enabled && !string.IsNullOrWhiteSpace(_settings.Password);

    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        return View("~/Views/Admin/Login.cshtml", new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Admin/Login.cshtml", model);
        }

        if (!FixedTimeEquals(model.Password, _settings.Password))
        {
            ModelState.AddModelError(nameof(model.Password), "Niepoprawne hasło.");
            return View("~/Views/Admin/Login.cshtml", model);
        }

        var claims = new[]
        {
            new Claim("admin", "true"),
            new Claim(ClaimTypes.Name, "admin")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            });

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Admin");
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left ?? string.Empty);
        var rightBytes = Encoding.UTF8.GetBytes(right ?? string.Empty);

        if (leftBytes.Length != rightBytes.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
