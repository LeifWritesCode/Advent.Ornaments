using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

internal class Submission
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime DateTime { get; set; }

    [Required]
    public string Answer { get; set; }

    [Required]
    public Response Response { get; set; }

    public Submission(int id, DateTime dateTime, string answer, Response response, Identity identity, Challenge challenge)
    {
        Id = id;
        DateTime = dateTime;
        Answer = answer;
        Response = response;
        Identity = identity;
        Challenge = challenge;
    }

    [Required]
    public Identity Identity { get; set; }

    [Required]
    public Challenge Challenge { get; set; }
}
