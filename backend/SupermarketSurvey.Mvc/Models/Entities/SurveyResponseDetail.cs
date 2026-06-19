namespace SupermarketSurvey.Mvc.Models.Entities;

public class SurveyResponseDetail
{
    public int Id { get; set; }
    public int SurveyResponseId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerOptionId { get; set; }

    public SurveyResponse SurveyResponse { get; set; } = null!;
    public Question Question { get; set; } = null!;
    public AnswerOption AnswerOption { get; set; } = null!;
}
