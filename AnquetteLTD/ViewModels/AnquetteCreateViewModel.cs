using System.ComponentModel.DataAnnotations;

namespace AnquetteLTD.ViewModels;

public class AnquetteCreateViewModel
{
    [Required, MaxLength(1_000)]
    public string Description { get; set; } = string.Empty;
    public bool IsMultipleChoice { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool IsEnabled { get; set; } = false;
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public List<string> Answers { get; set; } = new();
}