using Ornaments.Internals;

namespace Ornaments.App;

public static class OrnamentApp
{
    public static IOrnamentAppBuilder CreateDefaultBuilder() => new CommandLineAdventAppBuilder();

    public static IOrnamentApp CreateDefault() => CreateDefaultBuilder().Build();
}
