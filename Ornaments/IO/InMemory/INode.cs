namespace Ornaments.IO.InMemory;

public interface INode
{
    string Name { get; }

    int Size { get; }

    int SizeOnDisk { get; }

    INode? Parent { get; }
}
