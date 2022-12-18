namespace Ornaments.IO.InMemory;

public class DirectoryNode : INode
{
    public string Name { get; init; }

    public int Size => 0;

    public int SizeOnDisk => Children.Sum(x => x.SizeOnDisk);

    public IList<INode> Children { get; init; }

    public INode? Parent { get; init; }

    public DirectoryNode(string name, INode parent)
    {
        Name = name;
        Parent = parent;
        Children = new List<INode>();
    }
}
