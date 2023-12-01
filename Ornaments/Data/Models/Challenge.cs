using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Ornaments.Data.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class Challenge
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public int Day { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public ICollection<Submission> Submissions { get; set; }

    public ICollection<Input> Inputs { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
