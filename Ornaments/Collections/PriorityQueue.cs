using Ornaments.Extensions;

namespace Ornaments.Collections;

/// <summary>
/// Represents a prioritised first-in, first-out collection of objects.
/// </summary>
/// <typeparam name="Titem">Specifies the type of elements in the queue.</typeparam>
/// <remarks>
/// The <see cref="PriorityQueue{T}"/> type is FIFO on a per-priority basis.
/// E.g. an item enqueued at priority 3 will be dequeued after others of priority 3
/// but before any of priority 4.
/// </remarks>
public class PriorityQueue<T>
{
    private readonly List<(T value, int priority)> queue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public PriorityQueue()
    {
        queue = new List<(T, int)>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PriorityQueue{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="values">The collection whose elements are copied to the new <see cref="PriorityQueue{T}"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
    public PriorityQueue(params (T v, int p)[] values)
        : this()
    {
        ArgumentNullException.ThrowIfNull(nameof(values));
        foreach (var (v, p) in values)
        {
            Enqueue(v, p);
        }
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="PriorityQueue{T}"/>.
    /// </summary>
    /// <returns>The object that is removed from the beginning of the <see cref="PriorityQueue{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{T}"/> is empty.</exception>
    public T Dequeue()
    {
        if (queue.IsEmpty())
            throw new InvalidOperationException();

        var highest = queue.First();
        queue.Remove(highest);
        return highest.value;
    }

    /// <summary>
    /// Returns the object at the beginning of the <see cref="PriorityQueue{T}"/> without removing it.
    /// </summary>
    /// <returns>The object at the beginning of the <see cref="PriorityQueue{T}"/>.</returns>
    /// <exception cref="InvalidOperationException">The <see cref="PriorityQueue{T}"/> is empty.</exception>
    public T Peek()
    {
        if (queue.IsEmpty())
            throw new InvalidOperationException();

        return queue.First().value;
    }

    /// <summary>
    /// Adds a prioritised object to the end of the <see cref="PriorityQueue{T}"/>.
    /// </summary>
    /// <param name="value">The object to add to the <see cref="PriorityQueue{T}"/>. The value can be null for reference types.</param>
    /// <param name="priority">The priority of the object.</param>
    public void Enqueue(T value, int priority)
    {
        var index = 0;
        foreach (var item in queue)
        {
            // queue - fifo
            // new item should come after anything of the same priority
            // but before anything of lower priority
            if (priority < item.priority)
            {
                index = queue.IndexOf(item);
                break;
            }
        }
        queue.Insert(index, new(value, priority));
    }
}
