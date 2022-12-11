using Ornaments.App;
using Ornaments.Solutions;

namespace ManualTestHarness;

[RegisterOrnament("SomeChallenge", 2022, 1)]
internal sealed class SomeSolution : ISolution
{
    public SomeSolution() {}

    public async Task<object> DoPartOneAsync(ISolutionContext solutionContext)
    {
        return new();
    }

    public async Task<object> DoPartTwoAsync(ISolutionContext solutionContext)
    {
        return new();
    }

    public bool TryParse(string input, out object parsed)
    {
        parsed = new();
        return true;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        OrnamentApp.CreateDefault().RunAsync(args).GetAwaiter().GetResult();
    }
}