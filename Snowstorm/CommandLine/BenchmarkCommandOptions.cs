namespace Snowstorm.CommandLine;

internal class BenchmarkCommandOptions
{
    public int Year { get; init; }

    public int Day { get; init; }

    public int Runs { get; init; }

    public BenchmarkCommandOptions(int year, int day, int runs)
    {
        Year = year;
        Day = day;
        Runs = runs;
    }
}
