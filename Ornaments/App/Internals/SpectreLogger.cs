using Spectre.Console;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;

namespace Ornaments.App.Internals;

internal class SpectreLogger<TClass> : ILogger<TClass>
    where TClass : class
{
    private static readonly DateTime startTime = Process.GetCurrentProcess().StartTime.ToUniversalTime();

    private void WriteMessage(string color, string level, string message, params object[] args)
    {
        AnsiConsole.MarkupLine($"[silver]+{DateTime.UtcNow - startTime} [{color}][[{level}]][/] {typeof(TClass).Name}: {message}[/]", args);
    }

    public void Error(string message, params object[] args)
    {
        WriteMessage("bold red", "error", message, args);
    }

    public void Information(string message, params object[] args)
    {
        WriteMessage("bold white", "info ", message, args);
    }

    public void Verbose(string message, params object[] args)
    {
        AnsiConsole.MarkupLine($"[grey]+{DateTime.UtcNow - startTime} [[debug]] {typeof(TClass).Name}: {message}[/]", args);
    }

    public void Warning(string message, params object[] args)
    {
        WriteMessage("bold orange1", "warn ", message, args);
    }
}
