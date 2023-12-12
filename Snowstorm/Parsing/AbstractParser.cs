namespace Snowstorm.Parsing;

public abstract class AbstractParser<Tparsed> : IParser<Tparsed> where Tparsed : class?
{
    private Tparsed? _parsed = null;

    public string Raw { get; }

    public Tparsed Parsed
    {
        get
        {
            if (_parsed is not null) return _parsed;
            _parsed = Parse();
            return _parsed;
        }
    }

    public AbstractParser(string raw)
    {
        Raw = raw;
    }

    protected abstract Tparsed Parse();
}
