namespace SupermarketSurvey.Mvc.Models.ViewModels;

public class QuestionnaireViewModel
{
    public int SurveyId { get; set; }
    public string SurveyTitle { get; set; } = string.Empty;
    public List<QuestionItemViewModel> Questions { get; set; } = new();
}

public class QuestionItemViewModel
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public List<OptionItemViewModel> Options { get; set; } = new();
    public int SelectedOptionId { get; set; }
}

public class OptionItemViewModel
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
