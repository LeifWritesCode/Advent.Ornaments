using Ornaments.Extensions;
using System.Diagnostics;

namespace Ornaments.Internals;

internal class TokenOptions
{
    public const string Section = "tokens";

    public string GitHub { get; set; }

    public string Google { get; set; }

    public string Reddit { get; set; }

    public string Twitter { get; set; }

    public TokenOptions(string gitHub, string google, string reddit, string twitter)
    {
        GitHub = gitHub;
        Google = google;
        Reddit = reddit;
        Twitter = twitter;
    }

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
