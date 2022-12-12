namespace Ornaments.Internals;

internal class OrnamentsOptions
{
    public const string Section = "ornaments";

    public string Database { get; set; } = "ornament.db";

    public string EmailAddress { get; set; } = string.Empty;

    public string SourceCodeUri { get; set; } = string.Empty;
}
