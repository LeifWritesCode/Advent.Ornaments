using Ornaments.Graphs;
using System.Diagnostics;

namespace Ornaments.Algorithms;

/// <summary>
/// Implementation of the A* path finding algorithm.
/// </summary>
public class AStar
{
    private readonly WeightedGraph weightedGraph;
    private readonly int width;

    public AStar(WeightedGraph weightedGraph, int width)
    {
        this.weightedGraph = weightedGraph;
        this.width = width;
    }

    private static IEnumerable<int> Reconstruct(Dictionary<int, int> path, int current)
    {
        var result = new List<int>();
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

    public IEnumerable<int> Plot(Func<(int p, int q, int w), bool> predicate, int start, int end)
    {
        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        var openSet = new HashSet<int>() { start };

        // For node n, path[n] is the node immediately preceding it on the cheapest path from start
        var path = new Dictionary<int, int>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        var gScores = new Dictionary<int, int>() { { start, 0 } };

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        var fScores = new Dictionary<int, int>() { { start, weightedGraph.ManhattanHeuristic(start, end, width) } };

        while (openSet.Any())
        {
            var current = openSet.OrderBy(x => fScores[x]).First();
            if (current == end)
            {
                // finished!
                return Reconstruct(path, current);
            }

            openSet.Remove(current);
            var neighbours = weightedGraph.Neighbours(current);
            foreach (var neighbour in neighbours)
            {
                // ignore if an invalid next step
                var weight = weightedGraph.W[(current, neighbour)];
                if (!predicate((current, neighbour, weight)))
                    continue;

                // todo compute tentative gScore
                var tentativeGScore = gScores[current] + 1;

                // ensure g score is infinity at first use
                if (!gScores.ContainsKey(neighbour))
                    gScores.Add(neighbour, int.MaxValue);

                // ensure f score is infinity at first use
                if (!fScores.ContainsKey(neighbour))
                    fScores.Add(neighbour, int.MaxValue);

                if (tentativeGScore < gScores[neighbour])
                {
                    // add link into path
                    path[neighbour] = current;
                    gScores[neighbour] = tentativeGScore;
                    fScores[neighbour] = tentativeGScore + weightedGraph.ManhattanHeuristic(neighbour, end, width);

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        throw new UnreachableException();
    }
}
