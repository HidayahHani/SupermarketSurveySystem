namespace SupermarketSurvey.Mvc.Models.Entities;

public class Question
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderNumber { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Survey Survey { get; set; } = null!;
    public ICollection<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    public ICollection<SurveyResponseDetail> SurveyResponseDetails { get; set; } = new List<SurveyResponseDetail>();
}
