using Ornaments.App.Internals;

namespace Ornaments.App;

public static class OrnamentsApp
{
    public static IOrnamentsAppBuilder CreateDefaultBuilder() => new CommandLineOrnamentsAppBuilder();

    public static IOrnamentsApp CreateDefault() => CreateDefaultBuilder().Build();
}
