namespace AnquetteLTD.ViewModels;

public class AnquetteEditViewModel
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsMultipleChoice { get; set; }

    public bool AllowAnonymous { get; set; }
    public bool IsEnabled { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public List<AnswerItemViewModel> Answers { get; set; } = new();
}