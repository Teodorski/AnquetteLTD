namespace AnquetteLTD.ViewModels;

public class AnquetteVoteViewModel
{
    public Guid AnquetteId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsMultipleChoice { get; set; }
    public bool IsOpen { get; set; } // computed once for the view

    public List<AnswerOptionVm> Options { get; set; } = new();
    public List<int> SelectedAnswerIds { get; set; } = new(); // bound on POST
}

public record AnswerOptionVm(int Id, string Value);