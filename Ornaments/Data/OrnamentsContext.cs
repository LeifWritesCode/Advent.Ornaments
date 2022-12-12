using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Ornaments.Data.Models;
using Ornaments.Internals;

namespace Ornaments.Data
{
    internal class OrnamentsContext : DbContext
    {
        private readonly OrnamentsOptions ornamentOptions;

        public DbSet<Challenge> Challenges { get; set; }

        public DbSet<Submission> Submissions { get; set; }

        public DbSet<Identity> Identities { get; set; }

        public DbSet<Input> Inputs { get; set; }

        public OrnamentsContext(DbContextOptions<OrnamentsContext> options, IOptions<OrnamentsOptions> ornamentOptions)
            : base(options)
        {
            this.ornamentOptions = ornamentOptions.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite($"Data Source={ornamentOptions.Database}");
        }
    }
}
