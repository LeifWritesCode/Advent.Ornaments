namespace Snowstorm.Parsing;

/// <summary>
/// A contract that all parsers implemented.
/// </summary>
/// <typeparam name="Tparsed">The type of the output of this parser.</typeparam>
internal interface IParser<Tparsed>
{
    /// <summary>
    /// The raw, unparsed input.
    /// </summary>
    string Raw { get; }

    /// <summary>
    /// The result of parsing <see cref="Raw"/>.
    /// </summary>
    Tparsed Parsed { get; }
}
