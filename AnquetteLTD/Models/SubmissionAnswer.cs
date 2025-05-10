using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnquetteLTD.Models;

public class SubmissionAnswer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Guid SubmissionId { get; set; }
    public int AnswerId { get; set; }

    public virtual AnquetteSubmission? Submission { get; set; }
    public virtual Answer? Answer { get; set; }
}