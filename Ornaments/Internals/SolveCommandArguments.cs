namespace Ornaments.Internals;

internal class SolveCommandArguments
{
    public int Year { get; init; }

    public int Day { get; init; }

    public bool DryRun { get; init; }

    public int Timeout { get; init; }

    public SolveCommandArguments(int year, int day, bool dryRun, int timeout)
    {
        Year = year;
        Day = day;
        DryRun = dryRun;
        Timeout = timeout;
    }
}
