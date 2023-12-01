namespace Ornaments.App.Internals;

internal class BenchmarkCommandArguments
{
    public int Year { get; init; }

    public int Day { get; init; }

    public int Runs { get; init; }

    public BenchmarkCommandArguments(int year, int day, int runs)
    {
        Year = year;
        Day = day;
        Runs = runs;
    }
}
