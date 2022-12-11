using Ornaments.Internals;

namespace Ornaments.App;

public static class AdventApp
{
    public static IAdventAppBuilder CreateDefaultBuilder() => new CommandLineAdventAppBuilder();

    public static IAdventApp CreateDefault() => CreateDefaultBuilder().Build();
}
