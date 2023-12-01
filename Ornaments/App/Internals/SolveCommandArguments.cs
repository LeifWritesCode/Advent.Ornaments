namespace Ornaments.App.Internals;

internal class SolveCommandArguments
{
    public int Year { get; init; }

    public int Day { get; init; }

    public bool DryRun { get; init; }

    public IEnumerable<TokenType> TokenTypes { get; init; }

    public int Timeout { get; init; }

    public SolveCommandArguments(int year, int day, bool dryRun, IEnumerable<TokenType> tokenTypes, int timeout)
    {
        Year = year;
        Day = day;
        DryRun = dryRun;
        TokenTypes = tokenTypes;
        Timeout = timeout;
    }
}
