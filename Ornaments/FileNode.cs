namespace Ornaments;

public class FileNode : INode
{
    public string Name {  get; init; }

    public int Size { get; init; }

    public int SizeOnDisk => Size;

    public INode? Parent { get; init; }

    public FileNode(string name, int size, INode parent)
    {
        Name = name;
        Size = size;
        Parent = parent;
    }
}
