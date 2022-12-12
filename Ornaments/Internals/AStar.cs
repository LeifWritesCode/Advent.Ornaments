using Ornaments.Algorithms.PathFinding;
using System.Diagnostics;

namespace Ornaments.Internals;

/// <summary>
/// Implementation of the A* path finding algorithm.
/// </summary>
internal class AStar : IPathFindingAlgorithm
{
    /// <summary>
    /// Gets the manhattan distance between two given nodes.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="from">The current node.</param>
    /// <param name="to">The target node.</param>
    /// <param name="cost">The cost from origin to the current node.</param>
    /// <returns>The manhattan distance between from and to.</returns>
    private static int Heuristic<T>(Cell<T> from, Cell<T> to)
    {
        return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
    }

    private static IEnumerable<Cell<T>> Neighbours<T>(Grid<T> grid, Cell<T> current)
    {
        return new[]
        {
            grid.Above(current).FirstOrDefault(),
            grid.Below(current).FirstOrDefault(),
            grid.Aleft(current).FirstOrDefault(),
            grid.Aright(current).FirstOrDefault()
        }.Where(x => x is not null).Select(x => x!);
    }

    private static IEnumerable<Cell<T>> Reconstruct<T>(Dictionary<Cell<T>, Cell<T>> path, Cell<T> current)
    {
        var result = new List<Cell<T>>();
        while (true)
        {
            if (!path.ContainsKey(current))
                break;

            var next = path[current];
            result.Add(current);
            current = next;
        }
        return result;
    }

    public IEnumerable<Cell<T>> Find<T>(Grid<T> domain, Func<Cell<T>, Cell<T>, bool> predicate, (int x, int y) origin, (int x, int y) target)
    {
        var start = domain.Cell(origin.x, origin.y);
        var end = domain.Cell(target.x, target.y);

        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        var openSet = new HashSet<Cell<T>>() { start };

        // For node n, path[n] is the node immediately preceding it on the cheapest path from start
        var path = new Dictionary<Cell<T>, Cell<T>>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        var gScores = new Dictionary<Cell<T>, int>() { { start, 0 } };

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        var fScores = new Dictionary<Cell<T>, int>() { { start, Heuristic(start, end) } };

        while (openSet.Any())
        {
            var current = openSet.OrderBy(x => fScores[x]).First();
            if (current.X == target.x && current.Y == target.y)
            {
                // finished!
                return Reconstruct(path, current);
            }

            openSet.Remove(current);
            var neighbours = Neighbours(domain, current);
            foreach (var neighbor in neighbours)
            {
                // ignore if an invalid next step
                if (!predicate(current, neighbor))
                    continue;

                // todo compute tentative gScore
                var tentativeGScore = gScores[current] + 1;

                // ensure g score is infinity at first use
                if (!gScores.ContainsKey(neighbor))
                    gScores.Add(neighbor, int.MaxValue);

                // ensure f score is infinity at first use
                if (!fScores.ContainsKey(neighbor))
                    fScores.Add(neighbor, int.MaxValue);

                if (tentativeGScore < gScores[neighbor])
                {
                    // add link into path
                    path[neighbor] = current;
                    gScores[neighbor] = tentativeGScore;
                    fScores[neighbor] = tentativeGScore + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        throw new UnreachableException();
    }
}
