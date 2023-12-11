using Microsoft.Extensions.DependencyInjection;
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
    private readonly Tparser _parser;

    public AbstractSolution(IServiceProvider serviceProvider)
    {
        _parser = serviceProvider.GetRequiredService<Tparser>();
    }

    public abstract string PartOne();

    public abstract string PartTwo();
}
