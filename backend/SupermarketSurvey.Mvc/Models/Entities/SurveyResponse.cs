namespace SupermarketSurvey.Mvc.Models.Entities;

public class SurveyResponse
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public int RespondentId { get; set; }
    public int OutletId { get; set; }
    public int MerchandiserId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Survey Survey { get; set; } = null!;
    public Respondent Respondent { get; set; } = null!;
    public Outlet Outlet { get; set; } = null!;
    public Merchandiser Merchandiser { get; set; } = null!;
    public ICollection<SurveyResponseDetail> Details { get; set; } = new List<SurveyResponseDetail>();
}
