using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SupermarketSurvey.Mvc.Data;
using SupermarketSurvey.Mvc.Models.ViewModels;

namespace SupermarketSurvey.Mvc.Controllers;

public class AccountController(AppDbContext db) : Controller
{
    // Root / → redirect to Login
    public IActionResult Index() => RedirectToAction("Login");

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var merchandiser = await db.Merchandisers
            .Include(m => m.AssignedOutlet)
            .FirstOrDefaultAsync(m => m.Email == model.Email && m.IsActive);

        if (merchandiser is null || !BCrypt.Net.BCrypt.Verify(model.Password, merchandiser.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, merchandiser.Id.ToString()),
            new(ClaimTypes.Name,           merchandiser.Name),
            new(ClaimTypes.Email,          merchandiser.Email),
            new("EmployeeId",              merchandiser.EmployeeId),
            new("OutletId",                merchandiser.AssignedOutletId?.ToString() ?? ""),
            new("OutletName",              merchandiser.AssignedOutlet?.OutletName ?? ""),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("Index", "Dashboard");
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
