using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Ornaments.App.Internals;

namespace Ornaments.Data;

internal class OrnamentsContextFactory : IDesignTimeDbContextFactory<OrnamentsContext>
{
    public OrnamentsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrnamentsContext>();
        var ornamentOptions = Options.Create(new OrnamentsOptions());
        return new OrnamentsContext(optionsBuilder.Options, ornamentOptions);
    }
}
