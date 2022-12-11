using System.Numerics;

namespace Ornaments.Extensions;

public static class NumericExtensions
{
    public static bool IsNotInRange<TSelf>(this TSelf number, TSelf min, TSelf max)
        where TSelf : INumber<TSelf>
    {
        return number < min || number > max;
    }

    public static bool IsInRange<TSelf>(this TSelf number, TSelf min, TSelf max)
        where TSelf : INumber<TSelf>
    {
        return number >= min && number <= max;
    }
}
