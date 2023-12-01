using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

internal class Submission
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime DateTime { get; set; }

    [Required] public string Answer { get; set; } = string.Empty;

    [Required]
    public Response Response { get; set; }

    [Required]
    public Challenge? Challenge { get; set; }

    [Required]
    public Input? Input { get; set; }
}
