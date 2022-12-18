using Ornaments.Graphs;

namespace Ornaments.Algorithms;

public static class GraphExtensions
{

    /// <summary>
    /// Gets the manhattan distance between two given nodes on a graph representing a grid.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="from">The current node.</param>
    /// <param name="to">The target node.</param>
    /// <param name="cost">The cost from origin to the current node.</param>
    /// <returns>The manhattan distance between from and to.</returns>
    public static int ManhattanHeuristic(this IGraph graph, int p, int q, int width)
    {
        var (x1, y1) = (p % width, p / width);
        var (x2, y2) = (q % width, q / width);
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}
