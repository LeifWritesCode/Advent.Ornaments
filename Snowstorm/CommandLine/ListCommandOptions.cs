namespace Snowstorm.CommandLine;

internal class ListCommandOptions
{
    public IEnumerable<int> Years { get; init; }

    public ListCommandOptions(IEnumerable<int> years)
    {
        Years = years;
    }
}
