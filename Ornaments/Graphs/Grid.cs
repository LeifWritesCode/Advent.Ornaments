using System.ComponentModel;
using System.Diagnostics;

namespace Ornaments.Graphs;

/// <summary>
/// Represents an arbitrary NxM grid, with the origin in the top left corner.
/// </summary>
/// <typeparam name="Tvalue">The grid-cell value.</typeparam>
public class Grid<Tvalue>
{
    private readonly ICollection<Cell<Tvalue>> cells;

    public int Width { get; init; }

    public int Height { get; init; }

    public Grid(string[] grid)
    {
        ArgumentNullException.ThrowIfNull(grid);
        Trace.Assert(grid.Length > 0);

        var converter = TypeDescriptor.GetConverter(typeof(Tvalue));
        ArgumentNullException.ThrowIfNull(converter);

        Width = grid.First().Length;
        Height = grid.Length;

        cells = new List<Cell<Tvalue>>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var value = (Tvalue?)converter.ConvertFrom(grid[y][x].ToString());
                ArgumentNullException.ThrowIfNull(value);
                cells.Add(new Cell<Tvalue>(x, y, value));
            }
        }
    }

    /// <summary>
    /// Get a cell by coordinates.
    /// </summary>
    /// <param name="x">The east-west location of the cell.</param>
    /// <param name="y">The north-south location of the cell.</param>
    /// <returns>The cell at (x,y).</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="x"/> or <paramref name="y"/> were out of bounds.
    /// </exception>
    public Cell<Tvalue> Cell(int x, int y)
    {
        var cell = cells.FirstOrDefault(c => c.X == x && c.Y == y);
        ArgumentNullException.ThrowIfNull(cell);
        return cell!;
    }

    public Cell<Tvalue> First(Tvalue value)
    {
        return cells.First(x => x.Value?.Equals(value) ?? false);
    }

    public IEnumerable<Cell<Tvalue>> Where(Func<Cell<Tvalue>, bool> predicate)
    {
        return cells.Where(predicate);
    }

    public IEnumerable<Cell<Tvalue>> Adjacents(Cell<Tvalue> cell)
    {
        return new[]
        {
            Above(cell).FirstOrDefault(),
            Below(cell).FirstOrDefault(),
            Aleft(cell).FirstOrDefault(),
            Aright(cell).FirstOrDefault()
        }.Where(x => x is not null).Select(x => x!);
    }

    /// <summary>
    /// Get all cells immediately due west of the given cell.
    /// </summary>
    /// <param name="cell">The reference cell.</param>
    /// <returns>All cells satisfying aX == refY && aX < refX, ordered closest-to-furthest.</returns>
    public IEnumerable<Cell<Tvalue>> Aleft(Cell<Tvalue> cell)
    {
        var result = cells.Where(x => x.Y == cell.Y && x.X < cell.X).OrderByDescending(x => x.X);
        return result;
    }

    /// <summary>
    /// Get all cells immediately due east of the given cell.
    /// </summary>
    /// <param name="cell">The reference cell.</param>
    /// <returns>All cells satisfying aY == refY && aX > refX, ordered closest-to-furthest.</returns>
    public IEnumerable<Cell<Tvalue>> Aright(Cell<Tvalue> cell)
    {
        var result = cells.Where(x => x.Y == cell.Y && x.X > cell.X).OrderBy(x => x.X);
        return result;
    }

    /// <summary>
    /// Get all cells immediately due north of the given cell.
    /// </summary>
    /// <param name="cell">The reference cell.</param>
    /// <returns>All cells satisfying aX == refX && aY < refY, ordered closest-to-furthest.</returns>
    public IEnumerable<Cell<Tvalue>> Above(Cell<Tvalue> cell)
    {
        var result = cells.Where(x => x.X == cell.X && x.Y < cell.Y).OrderByDescending(x => x.Y);
        return result;
    }

    /// <summary>
    /// Get all cells immediately due south of the given cell.
    /// </summary>
    /// <param name="cell">The reference cell.</param>
    /// <returns>All cells satisfying aX == refX && aY > refY, ordered closest-to-furthest.</returns>
    public IEnumerable<Cell<Tvalue>> Below(Cell<Tvalue> cell)
    {
        var result = cells.Where(x => x.X == cell.X && x.Y > cell.Y).OrderBy(x => x.Y);
        return result;
    }
}
