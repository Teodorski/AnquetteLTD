namespace AnquetteLTD.ViewModels;

public class AnswerItemViewModel
{
    /// <summary>Database key. 0 ⇒ brand-new answer.</summary>
    public int Id { get; set; }

    public string Value { get; set; } = string.Empty;

    /// <summary>Hidden flag—true when user removed the row client-side.</summary>
    public bool IsDeleted { get; set; } = false;
}