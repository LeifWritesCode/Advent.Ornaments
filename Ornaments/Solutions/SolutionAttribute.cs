namespace Ornaments.Solutions;

[AttributeUsage(AttributeTargets.Class)]
public class SolutionAttribute : Attribute
{
    public string Name { get; init; }

    public int Year { get; init; }

    public int Day { get; init; }

    public SolutionAttribute(string name, int year, int day)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"Name must not be empty.", nameof(name));
        if (year != Math.Clamp(year, 2015, 2022))
            throw new ArgumentException($"Year must be a value between 2015 and 2022 inclusive.", nameof(year));
        if (day != Math.Clamp(day, 1, 25))
            throw new ArgumentException($"Day must be a value between 1 and 25 inclusive.", nameof(day));

        Name = name;
        Year = year;
        Day = day;
    }
}
