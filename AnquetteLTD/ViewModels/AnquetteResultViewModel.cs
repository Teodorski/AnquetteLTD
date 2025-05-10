namespace AnquetteLTD.ViewModels;

public class AnquetteResultsViewModel
{
    public Guid AnquetteId { get; set; }
    public string Description { get; set; } = string.Empty;

    public List<string> Labels { get; set; } = new();   // answer texts
    public List<int> Counts { get; set; } = new();   // vote totals

    public List<VoterGroupVm> Voters { get; set; } = new();
}

public class VoterGroupVm
{
    public string Display { get; set; } = string.Empty;  // "Alice", "Guest"
    public int Votes { get; set; }
}