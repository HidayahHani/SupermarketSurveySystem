using System.ComponentModel.DataAnnotations;

namespace SupermarketSurvey.Mvc.Models.ViewModels;

public class RespondentViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = string.Empty;

    [Required, Range(1, 120, ErrorMessage = "Enter a valid age.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Gender is required.")]
    public string Gender { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required(ErrorMessage = "Occupation is required.")]
    public string Occupation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Income range is required.")]
    public string IncomeRange { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Survey date is required.")]
    public string SurveyDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");

    public string? Notes { get; set; }
}
