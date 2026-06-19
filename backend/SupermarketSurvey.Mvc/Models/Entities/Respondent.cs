namespace SupermarketSurvey.Mvc.Models.Entities;

public class Respondent
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public string IncomeRange { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateOnly SurveyDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
}
