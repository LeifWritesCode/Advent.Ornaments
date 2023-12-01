namespace Ornaments.Graphs;

public class WeightedGraph : IWeightedGraph
{
    public IDictionary<(int p, int q), int> W { get; init; }

    public IList<(int p, int q)> E { get; init; }

    public WeightedGraph()
    {
        W = new Dictionary<(int p, int q), int>();
        E = new List<(int p, int q)>();
    }

    public bool AddEdge((int p, int q) e, int w)
    {
        var result = false;
        if (!E.Contains(e))
        {
            E.Add(e);
            W.Add(e, w);
            result = true;
        }
        return result;
    }

    public bool AddEdge((int p, int q) e)
    {
        return AddEdge(e, 0);
    }

    public bool Adjacent(int p, int q)
    {
        return E.Contains((p, q));
    }

    public IEnumerable<int> Neighbours(int p)
    {
        return E.Where(e => e.p == p).Select(e => e.q);
    }

    public bool RemoveEdge((int p, int q) e)
    {
        var result = false;
        if (E.Contains(e))
        {
            E.Remove(e);
            W.Remove(e);
            result = true;
        }
        return result;
    }
}

public class WeightedGraph<TVertex> : IWeightedGraph<TVertex>
{
    public IDictionary<(int p, int q), int> W { get; init; }

    public IList<TVertex> V { get; init; }

    public IList<(int p, int q)> E { get; init; }

    public WeightedGraph()
    {
        W = new Dictionary<(int p, int q), int>();
        V = new List<TVertex>();
        E = new List<(int p, int q)>();
    }

    public bool AddEdge((int p, int q) e, int w)
    {
        var result = false;
        if (!E.Contains(e))
        {
            E.Add(e);
            W.Add(e, w);
            result = true;
        }
        return result;
    }

    public bool AddEdge((int p, int q) e)
    {
        return AddEdge(e, 0);
    }

    public int AddVertex(TVertex value)
    {
        V.Add(value);
        return V.Count - 1;
    }

    public bool Adjacent(int p, int q)
    {
        return E.Contains((p, q));
    }

    public IEnumerable<int> Neighbours(int p)
    {
        return E.Where(e => e.p == p).Select(e => e.q);
    }

    public bool RemoveEdge((int p, int q) e)
    {
        var result = false;
        if (E.Contains(e))
        {
            E.Remove(e);
            W.Remove(e);
            result = true;
        }
        return result;
    }

    public bool RemoveVertex(int v)
    {
        var result = false;
        if (v >= 0 && v < V.Count)
        {
            V.RemoveAt(v);
            var edges = E.Where(e => e.p == v || e.q == v);
            foreach (var e in edges)
            {
                E.Remove(e);
                W.Remove(e);
            }
            result = true;
        }
        return result;
    }
}
