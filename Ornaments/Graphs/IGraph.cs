namespace Ornaments.Graphs;

// a simple graph where verticies are represented by edge endpoints
public interface IGraph
{
    // list of edges e
    IEnumerable<(int p, int q)> E { get; }

    // true if edge (p -> q) exists
    bool Adjacent(int p, int q);

    // all neighbours q where (p -> q) exists
    IEnumerable<int> Neighbours(int p);

    // add edge (p -> q, 0) if it doesn't already exist
    // return true if edges were modified
    bool AddEdge((int p, int q) e);

    // remove edge (p -> q) if it exists
    // return true if edges were modified
    bool RemoveEdge((int p, int q) e);
}

// a graph with non-trivial vertices
// V(x) can be an index into some other collection, or an object in its own right
public interface IGraph<TVertex> : IGraph
{
    // list of vertices v
    IEnumerable<TVertex> V { get; }

    // add a new vertex, returning its index
    int AddVertex(TVertex value);

    // remove the vertex v, additionally removing any edges that reference it
    // return true if the collections were changed
    bool RemoveVertex(int v);
}

// a simple weighted graph where verticies are represented by edge endpoints
public interface IWeightedGraph : IGraph
{
    // list of weights w such that edge (p -> q) has weight w
    IDictionary<(int p, int q), int> W { get; }

    // add edge (p  -> q, w) if it doesn't already exist
    // return true if edges were modified
    bool AddEdge((int p, int q) e, int w);
}

// a weighted graph with non-trivial vertices
// V(x) can be an index into some other collection, or an object in its own right
// no additional functionality here just implements the other two high level concepts
public interface IWeightedGraph<TVertex> : IWeightedGraph, IGraph<TVertex> { }
