namespace Ornaments.Algorithms.PathFinding;

public interface IPathFindingAlgorithm
{
    IEnumerable<Cell<T>> Find<T>(Grid<T> domain, Func<Cell<T>, Cell<T>, bool> predicate, (int x, int y) origin, (int x, int y) target);
}
