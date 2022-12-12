using System.ComponentModel.DataAnnotations;

namespace Ornaments.Data.Models;

internal class Challenge
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public Challenge(int id, string name, string description, IEnumerable<Submission> submissions)
    {
        Id = id;
        Name = name;
        Description = description;
        Submissions = submissions;
    }

    public IEnumerable<Submission> Submissions { get; set; }
}
