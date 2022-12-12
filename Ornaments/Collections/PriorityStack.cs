using Ornaments.Extensions;

namespace Ornaments.Collections;

/// <summary>
/// Represents a prioritised variable size last-in-first-out (LIFO) collection of instances of the same specified type.
/// </summary>
/// <typeparam name="Titem">Specifies the type of elements in the stack.</typeparam>
/// <remarks>
/// The <see cref="PriorityStack{T}"/> type is LIFO on a per-priority basis.
/// E.g. an item pushed with priority 3 will be popped before others of priority 3
/// but not before any of priority 2.
/// </remarks>
public class PriorityStack<T>
{
    private readonly IList<(T value, int priority)> stack;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityStack{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public PriorityStack()
    {
        stack = new List<(T, int)>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityStack{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="values">The collection whose elements are copied to the new <see cref="PriorityStack{T}"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
    public PriorityStack(params (T v, int p)[] values)
        : this()
    {
        ArgumentNullException.ThrowIfNull(nameof(values));
        foreach (var (v, p) in values)
        {
            Push(v, p);
        }
    }

    /// <summary>
    /// Removes and returns the object at the top of the <see cref="PriorityStack{T}"/>.
    /// </summary>
    /// <returns>The object removed from the top of the <see cref="PriorityStack{T}"/.></returns>
    /// <exception cref="InvalidOperationException">The <see cref="PriorityStack{T}"/> is empty.</exception>
    public T Pop()
    {
        if (stack.IsEmpty())
            throw new InvalidOperationException();

        var highest = stack.First();
        stack.Remove(highest);
        return highest.value;
    }

    /// <summary>
    /// eturns the object at the top of the <see cref="PriorityStack{T}"/> without removing it.
    /// </summary>
    /// <returns>The object at the top of the <see cref="PriorityStack{T}"/.></returns>
    /// <exception cref="InvalidOperationException">The <see cref="PriorityStack{T}"/> is empty.</exception>
    public T Peek()
    {
        if (stack.IsEmpty())
            throw new InvalidOperationException();

        return stack.First().value;
    }

    /// <summary>
    /// Inserts a prioritised object at the top of the <see cref="PriorityStack{T}"/>.
    /// </summary>
    /// <param name="value">The object to push onto the <see cref="PriorityStack{T}"/>. The value can be null for reference types.</param>
    /// <param name="priority">The priority of the object.</param>
    public void Push(T value, int priority)
    {
        var index = 0;
        foreach (var item in stack)
        {
            // stack - lifo
            // new item should come after anything of greater priority
            // but before anything of the same priority
            // and before anything of lower priority
            if (priority <= item.priority)
            {
                index = stack.IndexOf(item);
                break;
            }
        }
        stack.Insert(index, new(value, priority));
    }
}
