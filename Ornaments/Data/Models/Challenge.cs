using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Ornaments.Data.Models;

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

    public IEnumerable<Submission> Submissions { get; set; } = Enumerable.Empty<Submission>();

    public IEnumerable<Input> Inputs { get; set; } = Enumerable.Empty<Input>();
}
