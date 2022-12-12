namespace Ornaments.Internals;

internal class ListCommandArguments
{
    public IEnumerable<int> Years { get; set; }

    public ListCommandArguments(IEnumerable<int> years)
    {
        Years = years;
    }
}
