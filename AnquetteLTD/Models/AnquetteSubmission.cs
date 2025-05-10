using System.ComponentModel.DataAnnotations;

namespace AnquetteLTD.Models;

public class AnquetteSubmission
{
    [Key]
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    [Required]
    public Guid AnquetteId { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public Anquette? Anquette { get; set; }

    public virtual ICollection<SubmissionAnswer> SelectedAnswers { get; set; } = new List<SubmissionAnswer>();
}