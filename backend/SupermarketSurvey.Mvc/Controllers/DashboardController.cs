using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermarketSurvey.Mvc.Data;

namespace SupermarketSurvey.Mvc.Controllers;

[Authorize]
public class DashboardController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.SurveyCount  = await db.Surveys.CountAsync(s => s.IsActive);
        ViewBag.OutletCount  = await db.Outlets.CountAsync();
        ViewBag.ResponseCount = await db.SurveyResponses.CountAsync();
        ViewBag.Surveys      = await db.Surveys
                                    .Where(s => s.IsActive)
                                    .OrderByDescending(s => s.Id)
                                    .ToListAsync();
        return View();
    }
}
