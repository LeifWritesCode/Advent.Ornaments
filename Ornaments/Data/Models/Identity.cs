using Ornaments.Internals;
using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

internal class Identity
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public TokenType Provider { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public IEnumerable<Input> Inputs { get; set; } = Enumerable.Empty<Input>();
}
