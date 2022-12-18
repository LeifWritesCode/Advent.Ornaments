namespace Ornaments.Graphs;

public class Cell<Tvalue>
{
    public int X { get; set; }

    public int Y { get; set; }

    public Tvalue Value { get; set; }

    public Cell(int x, int y, Tvalue value)
    {
        X = x;
        Y = y;
        Value = value;
    }
}
