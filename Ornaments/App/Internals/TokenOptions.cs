using Ornaments.Extensions;
using System.Diagnostics;

namespace Ornaments.App.Internals;

internal class TokenOptions
{
    public const string Section = "tokens";

    public string GitHub { get; set; } = string.Empty;

    public string Google { get; set; } = string.Empty;

    public string Reddit { get; set; } = string.Empty;

    public string Twitter { get; set; } = string.Empty;

    public bool TryGet(TokenType tokenType, out string token)
    {
        token = tokenType switch
        {
            TokenType.GitHub => GitHub,
            TokenType.Google => Google,
            TokenType.Twitter => Twitter,
            TokenType.Reddit => Reddit,
            _ => throw new UnreachableException()
        };

        return token.HasValue();
    }
}
