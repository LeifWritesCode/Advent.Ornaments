using System.Numerics;

namespace Snowstorm.Extensions;

public static class NumberExtensions
{
    /// <summary>
    /// True if <see cref="number"/> satsifies <see cref="min"/> <= <see cref="number"/> <= <see cref="max"/>.
    /// </summary>
    /// <typeparam name="Tnumber">A numeric type.</typeparam>
    /// <param name="number">The number to test.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns></returns>
    public static bool IsInRange<Tnumber>(this Tnumber number, Tnumber min, Tnumber max)
        where Tnumber : INumber<Tnumber>
    {
        return min <= number && number <= max;
    }

    /// <summary>
    /// True if <see cref="number"/> does not satisfy <see cref="min"/> <= <see cref="number"/> <= <see cref="max"/>.
    /// </summary>
    /// <typeparam name="Tnumber">A numeric type.</typeparam>
    /// <param name="number">The number to test.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="max">The maximum.</param>
    /// <returns></returns>
    public static bool IsNotInRange<Tnumber>(this Tnumber number, Tnumber min, Tnumber max)
        where Tnumber : INumber<Tnumber>
    {
        return !number.IsInRange(min, max);
    }
}
