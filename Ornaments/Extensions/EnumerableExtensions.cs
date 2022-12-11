namespace Ornaments.Extensions;

public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    /// <typeparam name="Tvalue">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="self">A sequence to return elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="inclusive">If true, includes the element at which the test no longer passes.</param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> that contains the elements from the input sequence that occur up to and, if <paramref name="inclusive"/> is true, including the element at which the test no longer passes.
    /// </returns>
    public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool inclusive = false)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        foreach (var item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
            else if (inclusive)
            {
                yield return item;
                yield break;
            }
        }
    }
}
