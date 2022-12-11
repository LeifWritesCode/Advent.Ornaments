using Ornaments.Solutions;

namespace Ornaments.Internals;

internal static class SolutionExtensions
{
    public static async Task RunAsync(this ISolution solution)
    {
        await Task.CompletedTask;
    }
}
