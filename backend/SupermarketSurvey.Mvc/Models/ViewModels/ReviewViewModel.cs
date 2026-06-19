namespace SupermarketSurvey.Mvc.Models.ViewModels;

public class ReviewViewModel
{
    public string SurveyTitle { get; set; } = string.Empty;
    public string OutletName { get; set; } = string.Empty;
    public string MerchandiserName { get; set; } = string.Empty;
    public RespondentViewModel Respondent { get; set; } = new();
    public List<ReviewAnswerItem> Answers { get; set; } = new();
}

public class ReviewAnswerItem
{
    public int QuestionNo { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
}
