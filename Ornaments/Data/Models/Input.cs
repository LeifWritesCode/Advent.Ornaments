using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

internal class Input
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public Identity? Identity { get; set; }

    [Required]
    public Challenge? Challenge { get; set; }

    public IEnumerable<Submission> Submissions { get; set; } = Enumerable.Empty<Submission>();
}
