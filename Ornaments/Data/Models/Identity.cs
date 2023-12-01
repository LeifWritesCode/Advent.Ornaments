using Ornaments.App.Internals;
using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class Identity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public TokenType Provider { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public ICollection<Input> Inputs { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
