namespace Ornaments.Extensions;

public static class StringExtensions
{
    public static bool HasValue(this string self)
    {
        return !string.IsNullOrWhiteSpace(self);
    }

    public static bool HasNoValue(this string self)
    {
        return string.IsNullOrWhiteSpace(self);
    }

    public static int ToInt(this string self)
    {
        return int.Parse(self);
    }
}
