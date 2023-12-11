namespace Snowstorm.Parsing;

public static class StringExtensions
{
    public static string[] AsLines(this string input)
    {
        return input.Split(Environment.NewLine);
    }

    public static string[] AsBlocks(this string input)
    {
        return input.Split($"{Environment.NewLine}{Environment.NewLine}");
    }
}
