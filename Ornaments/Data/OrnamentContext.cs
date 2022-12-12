using Microsoft.EntityFrameworkCore;
using Ornaments.Data.Models;

namespace Ornaments.Data
{
    internal class OrnamentContext : DbContext
    {
        public DbSet<Challenge> Challenges { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Identity> Identities { get; set; }

        public OrnamentContext(DbContextOptions<OrnamentContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=jingle.bells");
        }
    }
}
