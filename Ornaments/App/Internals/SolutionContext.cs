using Ornaments.Solutions;

namespace Ornaments.App.Internals;

internal class SolutionContext : ISolutionContext
{
    public object Input { get; private set; }

    public SolutionContext(object input)
    {
        Input = input;
    }

    public T As<T>() where T : class
    {
        var result = Input as T;
        if (result is null)
        {
            throw new InvalidCastException($"Input is not a {typeof(T)}.");
        }
        return result;
    }
}