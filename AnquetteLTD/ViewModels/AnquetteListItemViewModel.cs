namespace AnquetteLTD.ViewModels;

public class AnquetteListItemViewModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AuthorUserName { get; set; } = string.Empty;
    public int AnswersCount { get; set; }
    public bool IsMultipleChoice { get; set; }
    public bool AllowAnonymous { get; set; }
    public bool IsEnabled { get; set; }
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public bool IsOpen { get; set; }
}