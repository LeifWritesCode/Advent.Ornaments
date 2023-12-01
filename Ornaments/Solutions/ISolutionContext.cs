namespace Ornaments.Solutions;

public interface ISolutionContext
{
    /// <summary>
    /// The processed input for the solution.
    /// </summary>
    object Input { get; }

    /// <summary>
    /// Get the <see cref="Input"/> as an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to cast as.</typeparam>
    /// <returns>The cast input.</returns>
    /// <exception cref="InvalidCastException">The input is not a <typeparamref name="T"/>.</exception>
    T As<T>() where T : class;
}
