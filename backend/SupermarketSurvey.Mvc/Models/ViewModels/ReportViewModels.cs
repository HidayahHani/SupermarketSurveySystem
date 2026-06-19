namespace SupermarketSurvey.Mvc.Models.ViewModels;

public class ParticipationReportViewModel
{
    public string SurveyTitle { get; set; } = string.Empty;
    public string OutletName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string MerchandiserName { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public DateTime? FirstSubmission { get; set; }
    public DateTime? LastSubmission { get; set; }
}

public class StatisticsReportViewModel
{
    public string SurveyTitle { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public List<SurveyListItemViewModel> Surveys { get; set; } = new();
    public List<QuestionStatViewModel> Questions { get; set; } = new();
}

public class SurveyListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class QuestionStatViewModel
{
    public int OrderNumber { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<OptionStatViewModel> Options { get; set; } = new();
}

public class OptionStatViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}
