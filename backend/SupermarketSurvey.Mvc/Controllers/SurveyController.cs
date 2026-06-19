using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using SupermarketSurvey.Mvc.Data;
using SupermarketSurvey.Mvc.Models.Entities;
using SupermarketSurvey.Mvc.Models.ViewModels;

namespace SupermarketSurvey.Mvc.Controllers;

[Authorize]
public class SurveyController(AppDbContext db) : Controller
{
    // ── Step 1: Select Outlet ─────────────────────────────
    [HttpGet]
    public async Task<IActionResult> SelectOutlet()
    {
        var outlets = await db.Outlets.OrderBy(o => o.City).ThenBy(o => o.OutletName).ToListAsync();
        return View(outlets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectOutlet(int outletId, string outletName)
    {
        HttpContext.Session.SetInt32("OutletId",   outletId);
        HttpContext.Session.SetString("OutletName", outletName);
        return RedirectToAction("SelectSurvey");
    }

    // ── Step 2: Select Survey ─────────────────────────────
    [HttpGet]
    public async Task<IActionResult> SelectSurvey()
    {
        if (HttpContext.Session.GetInt32("OutletId") is null)
            return RedirectToAction("SelectOutlet");

        var surveys = await db.Surveys
            .Where(s => s.IsActive)
            .Select(s => new { s.Id, s.Title, s.Description, QuestionCount = s.Questions.Count })
            .OrderByDescending(s => s.Id)
            .ToListAsync();

        ViewBag.OutletName = HttpContext.Session.GetString("OutletName");
        return View(surveys);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectSurvey(int surveyId, string surveyTitle)
    {
        HttpContext.Session.SetInt32("SurveyId",    surveyId);
        HttpContext.Session.SetString("SurveyTitle", surveyTitle);
        return RedirectToAction("Respondent");
    }

    // ── Step 3: Respondent Details ────────────────────────
    [HttpGet]
    public IActionResult Respondent()
    {
        if (HttpContext.Session.GetInt32("SurveyId") is null)
            return RedirectToAction("SelectSurvey");

        ViewBag.SurveyTitle = HttpContext.Session.GetString("SurveyTitle");
        ViewBag.OutletName  = HttpContext.Session.GetString("OutletName");
        return View(new RespondentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Respondent(RespondentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SurveyTitle = HttpContext.Session.GetString("SurveyTitle");
            ViewBag.OutletName  = HttpContext.Session.GetString("OutletName");
            return View(model);
        }

        HttpContext.Session.SetString("RespondentJson", JsonSerializer.Serialize(model));
        return RedirectToAction("Questions");
    }

    // ── Step 4: Questions ─────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Questions()
    {
        var surveyId = HttpContext.Session.GetInt32("SurveyId");
        if (surveyId is null) return RedirectToAction("SelectSurvey");

        var survey = await db.Surveys
            .Include(s => s.Questions.OrderBy(q => q.OrderNumber))
                .ThenInclude(q => q.AnswerOptions.OrderBy(ao => ao.OptionLabel))
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey is null) return RedirectToAction("SelectSurvey");

        var vm = new QuestionnaireViewModel
        {
            SurveyId    = survey.Id,
            SurveyTitle = survey.Title,
            Questions   = survey.Questions.Select(q => new QuestionItemViewModel
            {
                Id           = q.Id,
                QuestionText = q.QuestionText,
                OrderNumber  = q.OrderNumber,
                Options      = q.AnswerOptions.Select(ao => new OptionItemViewModel
                {
                    Id    = ao.Id,
                    Label = ao.OptionLabel,
                    Text  = ao.OptionText
                }).ToList()
            }).ToList()
        };

        ViewBag.OutletName = HttpContext.Session.GetString("OutletName");
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Questions(QuestionnaireViewModel model)
    {
        // Validate all questions answered
        if (model.Questions.Any(q => q.SelectedOptionId == 0))
        {
            ModelState.AddModelError(string.Empty, "Please answer all questions before continuing.");
            ViewBag.OutletName = HttpContext.Session.GetString("OutletName");
            // Re-populate options from session survey (questions come back without options)
            return RedirectToAction("Questions");
        }

        var answers = model.Questions.Select(q => new { q.Id, q.SelectedOptionId }).ToList();
        HttpContext.Session.SetString("AnswersJson", JsonSerializer.Serialize(answers));
        return RedirectToAction("Review");
    }

    // ── Step 5: Review ────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Review()
    {
        var surveyId   = HttpContext.Session.GetInt32("SurveyId");
        var respondentJson = HttpContext.Session.GetString("RespondentJson");
        var answersJson    = HttpContext.Session.GetString("AnswersJson");

        if (surveyId is null || respondentJson is null || answersJson is null)
            return RedirectToAction("SelectOutlet");

        var respondent = JsonSerializer.Deserialize<RespondentViewModel>(respondentJson)!;
        var answers    = JsonSerializer.Deserialize<List<AnswerSessionItem>>(answersJson)!;

        var survey = await db.Surveys
            .Include(s => s.Questions.OrderBy(q => q.OrderNumber))
                .ThenInclude(q => q.AnswerOptions)
            .FirstOrDefaultAsync(s => s.Id == surveyId);

        if (survey is null) return RedirectToAction("SelectSurvey");

        var optionLookup = survey.Questions
            .SelectMany(q => q.AnswerOptions)
            .ToDictionary(ao => ao.Id);

        var questionLookup = survey.Questions.ToDictionary(q => q.Id);

        var reviewAnswers = answers.Select(a =>
        {
            var opt = optionLookup.GetValueOrDefault(a.SelectedOptionId);
            var q   = questionLookup.GetValueOrDefault(a.Id);
            return new ReviewAnswerItem
            {
                QuestionNo   = q?.OrderNumber ?? 0,
                QuestionText = q?.QuestionText ?? "",
                OptionLabel  = opt?.OptionLabel ?? "",
                OptionText   = opt?.OptionText ?? ""
            };
        }).OrderBy(a => a.QuestionNo).ToList();

        var vm = new ReviewViewModel
        {
            SurveyTitle      = survey.Title,
            OutletName       = HttpContext.Session.GetString("OutletName") ?? "",
            MerchandiserName = User.Identity?.Name ?? "",
            Respondent       = respondent,
            Answers          = reviewAnswers
        };

        return View(vm);
    }

    // ── Step 6: Submit ────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit()
    {
        var surveyId       = HttpContext.Session.GetInt32("SurveyId");
        var outletId       = HttpContext.Session.GetInt32("OutletId");
        var merchandiserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var respondentJson = HttpContext.Session.GetString("RespondentJson");
        var answersJson    = HttpContext.Session.GetString("AnswersJson");

        if (surveyId is null || outletId is null || respondentJson is null || answersJson is null)
            return RedirectToAction("SelectOutlet");

        var respondentVm = JsonSerializer.Deserialize<RespondentViewModel>(respondentJson)!;
        var answers      = JsonSerializer.Deserialize<List<AnswerSessionItem>>(answersJson)!;

        DateOnly.TryParse(respondentVm.SurveyDate, out var surveyDate);

        var respondent = new Respondent
        {
            Name        = respondentVm.Name,
            Age         = respondentVm.Age,
            Gender      = respondentVm.Gender,
            Phone       = respondentVm.Phone,
            Occupation  = respondentVm.Occupation,
            IncomeRange = respondentVm.IncomeRange,
            Location    = respondentVm.Location,
            SurveyDate  = surveyDate,
            Notes       = respondentVm.Notes
        };
        db.Respondents.Add(respondent);
        await db.SaveChangesAsync();

        var response = new SurveyResponse
        {
            SurveyId       = surveyId.Value,
            RespondentId   = respondent.Id,
            OutletId       = outletId.Value,
            MerchandiserId = merchandiserId,
            SubmittedAt    = DateTime.UtcNow
        };
        db.SurveyResponses.Add(response);
        await db.SaveChangesAsync();

        db.SurveyResponseDetails.AddRange(answers.Select(a => new SurveyResponseDetail
        {
            SurveyResponseId = response.Id,
            QuestionId       = a.Id,
            AnswerOptionId   = a.SelectedOptionId
        }));
        await db.SaveChangesAsync();

        HttpContext.Session.SetInt32("SubmittedResponseId", response.Id);
        HttpContext.Session.SetString("SubmittedRespondentName", respondent.Name);

        // Clear workflow state but keep outlet for next survey
        var savedOutletId   = HttpContext.Session.GetInt32("OutletId");
        var savedOutletName = HttpContext.Session.GetString("OutletName");
        HttpContext.Session.Remove("SurveyId");
        HttpContext.Session.Remove("SurveyTitle");
        HttpContext.Session.Remove("RespondentJson");
        HttpContext.Session.Remove("AnswersJson");

        return RedirectToAction("Success");
    }

    // ── Step 7: Success ───────────────────────────────────
    [HttpGet]
    public IActionResult Success()
    {
        ViewBag.ResponseId     = HttpContext.Session.GetInt32("SubmittedResponseId");
        ViewBag.RespondentName = HttpContext.Session.GetString("SubmittedRespondentName");
        ViewBag.OutletName     = HttpContext.Session.GetString("OutletName");
        return View();
    }
}

// Used for JSON session storage
public record AnswerSessionItem(int Id, int SelectedOptionId);
