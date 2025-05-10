using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AnquetteLTD.Models;

public class Anquette
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    public virtual IdentityUser? User { get; set; }

    [Required, MaxLength(1_000)]
    public string Description { get; set; } = string.Empty;

    public bool IsMultipleChoice { get; set; } = false;

    public bool AllowAnonymous { get; set; } = true;

    public bool IsEnabled { get; set; } = false;
    public DateTimeOffset? StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    public virtual ICollection<AnquetteSubmission> Submissions { get; set; } = new List<AnquetteSubmission>();
}