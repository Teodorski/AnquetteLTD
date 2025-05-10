using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnquetteLTD.Models;
public class Answer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(500)]
    public string Value { get; set; } = string.Empty;

    [Required]
    public Guid AnquetteId { get; set; }

    public virtual Anquette? Anquette { get; set; }
}
