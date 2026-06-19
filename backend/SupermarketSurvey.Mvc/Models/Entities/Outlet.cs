namespace SupermarketSurvey.Mvc.Models.Entities;

public class Outlet
{
    public int Id { get; set; }
    public string OutletName { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Merchandiser> Merchandisers { get; set; } = new List<Merchandiser>();
    public ICollection<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
}
