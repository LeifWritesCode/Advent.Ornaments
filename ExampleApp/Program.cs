using Ornaments.App;
using Ornaments.Solutions;

namespace ManualTestHarness;

[RegisterOrnament("CalorieCounting", 2022, 1)]
internal sealed class CalorieCounting : ISolution
{
    public CalorieCounting() {}

    public async Task<object> DoPartOneAsync(ISolutionContext solutionContext)
    {
        await Task.CompletedTask;
        return solutionContext.As<string[]>()
            .Select(x => x.Split(Environment.NewLine))
            .Select(x => x.Aggregate(0, (a, s) => a + int.Parse(s)))
            .Max();
    }

    public async Task<object> DoPartTwoAsync(ISolutionContext solutionContext)
    {
        await Task.CompletedTask;
        return solutionContext.As<string[]>()
            .Select(x => x.Split(Environment.NewLine))
            .Select(x => x.Aggregate(0, (a, s) => a + int.Parse(s)))
            .OrderDescending()
            .Take(3)
            .Sum();
    }

    public bool TryParse(string input, out object parsed)
    {
        parsed = input.Trim()
            .ReplaceLineEndings()
            .Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);
        return true;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        OrnamentsApp.CreateDefault().RunAsync(args).GetAwaiter().GetResult();
    }
}