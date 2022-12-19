using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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

    public ICollection<Submission> Submissions { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
