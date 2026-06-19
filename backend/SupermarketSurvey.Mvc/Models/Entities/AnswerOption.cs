namespace SupermarketSurvey.Mvc.Models.Entities;

public class AnswerOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Question Question { get; set; } = null!;
    public ICollection<SurveyResponseDetail> SurveyResponseDetails { get; set; } = new List<SurveyResponseDetail>();
}
