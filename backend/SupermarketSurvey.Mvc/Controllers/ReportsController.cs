using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupermarketSurvey.Mvc.Data;
using SupermarketSurvey.Mvc.Models.ViewModels;

namespace SupermarketSurvey.Mvc.Controllers;

[Authorize]
public class ReportsController(AppDbContext db) : Controller
{
    // ── Report 1: Survey Participation Summary ────────────
    // Equivalent SQL (see database/reports.sql):
    //   SELECT s.Title, o.OutletName, o.City, m.Name,
    //          COUNT(sr.Id) AS TotalResponses,
    //          MIN(sr.SubmittedAt), MAX(sr.SubmittedAt)
    //   FROM SurveyResponses sr
    //   JOIN Surveys s      ON sr.SurveyId       = s.Id
    //   JOIN Outlets o      ON sr.OutletId        = o.Id
    //   JOIN Merchandisers m ON sr.MerchandiserId = m.Id
    //   GROUP BY s.Id, o.Id, m.Id
    //   ORDER BY s.Title, o.OutletName
    public async Task<IActionResult> Index()
    {
        var rows = await db.SurveyResponses
            .GroupBy(sr => new
            {
                sr.Survey.Title,
                sr.Outlet.OutletName,
                sr.Outlet.City,
                MerchandiserName = sr.Merchandiser.Name
            })
            .OrderBy(g => g.Key.Title)
            .ThenBy(g => g.Key.OutletName)
            .Select(g => new ParticipationReportViewModel
            {
                SurveyTitle      = g.Key.Title,
                OutletName       = g.Key.OutletName,
                City             = g.Key.City,
                MerchandiserName = g.Key.MerchandiserName,
                TotalResponses   = g.Count(),
                FirstSubmission  = g.Min(x => (DateTime?)x.SubmittedAt),
                LastSubmission   = g.Max(x => (DateTime?)x.SubmittedAt)
            })
            .ToListAsync();

        return View(rows);
    }

    // ── Report 2: Question Answer Statistics ─────────────
    // Equivalent SQL (see database/reports.sql):
    //   SELECT q.OrderNumber, q.QuestionText,
    //          ao.OptionLabel, ao.OptionText,
    //          COUNT(srd.Id) AS AnswerCount,
    //          ROUND(COUNT(srd.Id) * 100.0 / total_per_question, 2) AS Percentage
    //   FROM Questions q
    //   JOIN AnswerOptions ao ON ao.QuestionId = q.Id
    //   LEFT JOIN SurveyResponseDetails srd ON srd.AnswerOptionId = ao.Id
    //   WHERE q.SurveyId = @surveyId
    //   GROUP BY q.Id, ao.Id
    //   ORDER BY q.OrderNumber, ao.OptionLabel
    public async Task<IActionResult> Statistics(int? surveyId)
    {
        var surveys = await db.Surveys
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.Id)
            .Select(s => new SurveyListItemViewModel { Id = s.Id, Title = s.Title })
            .ToListAsync();

        var vm = new StatisticsReportViewModel { Surveys = surveys };

        if (surveyId.HasValue)
        {
            var survey = await db.Surveys.FindAsync(surveyId.Value);
            if (survey is not null)
            {
                vm.SurveyTitle    = survey.Title;
                vm.TotalResponses = await db.SurveyResponses.CountAsync(sr => sr.SurveyId == surveyId);

                var rawRows = await db.AnswerOptions
                    .Where(ao => ao.Question.SurveyId == surveyId)
                    .OrderBy(ao => ao.Question.OrderNumber)
                    .ThenBy(ao => ao.OptionLabel)
                    .Select(ao => new
                    {
                        ao.Question.OrderNumber,
                        ao.Question.QuestionText,
                        ao.OptionLabel,
                        ao.OptionText,
                        AnswerCount      = ao.SurveyResponseDetails.Count(d => d.SurveyResponse.SurveyId == surveyId),
                        TotalForQuestion = ao.Question.SurveyResponseDetails.Count(d => d.SurveyResponse.SurveyId == surveyId)
                    })
                    .ToListAsync();

                vm.Questions = rawRows
                    .GroupBy(r => new { r.OrderNumber, r.QuestionText })
                    .Select(g => new QuestionStatViewModel
                    {
                        OrderNumber  = g.Key.OrderNumber,
                        QuestionText = g.Key.QuestionText,
                        Options      = g.Select(r => new OptionStatViewModel
                        {
                            Label      = r.OptionLabel,
                            Text       = r.OptionText,
                            Count      = r.AnswerCount,
                            Percentage = r.TotalForQuestion == 0 ? 0
                                : Math.Round(r.AnswerCount * 100.0 / r.TotalForQuestion, 1)
                        }).ToList()
                    })
                    .OrderBy(q => q.OrderNumber)
                    .ToList();
            }
        }

        ViewBag.SelectedSurveyId = surveyId;
        return View(vm);
    }
}
