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
    public string Token { get; set; }

    public Identity(int id, TokenType provider, string token, IEnumerable<Submission> submissions)
    {
        Id = id;
        Provider = provider;
        Token = token;
        Submissions = submissions;
    }

    public IEnumerable<Submission> Submissions { get; set; }
}
