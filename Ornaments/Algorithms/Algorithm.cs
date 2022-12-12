using Ornaments.Algorithms.PathFinding;
using Ornaments.Internals;

namespace Ornaments.Algorithms;

public static class Algorithm
{
    public static IPathFindingAlgorithm Create(PathFindingAlgorithm algorithm)
    {
        return algorithm switch
        {
            PathFindingAlgorithm.AStar => new AStar(),
            _ => throw new NotSupportedException($"Unknown algorithm {algorithm}.")
        };
    }
}
