using Ornaments.App.Internals;

namespace Ornaments.Solutions;

public static class Context
{
    public static ISolutionContext Create(ISolution solution, string input)
    {
        ArgumentNullException.ThrowIfNull(solution);
        ArgumentException.ThrowIfNullOrEmpty(input);

        if (solution.TryParse(input, out var parsed))
        {
            return new SolutionContext(parsed);
        }

        throw new ArgumentException("Unable to parse input.");
    }
}
