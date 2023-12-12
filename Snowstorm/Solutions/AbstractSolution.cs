using Microsoft.Extensions.DependencyInjection;
using Snowstorm.Extensions;
using Snowstorm.Parsing;

namespace Snowstorm.Solutions;

internal class SomeParser : AbstractParser<string[]>
{
    public SomeParser(string raw) : base(raw) { }

    protected override string[] Parse()
    {
        return Raw.AsLines();
    }
}

public class SomeThingUsingParser
{
    public SomeThingUsingParser()
    {
        
    }
}

internal abstract class AbstractSolution<Tparser> : ISolution
    where Tparser : notnull
{
    protected readonly Tparser _parser;

    public AbstractSolution(Tparser parser)
    {
        _parser = parser;
    }

    public abstract string PartOne();

    public abstract string PartTwo();
}
