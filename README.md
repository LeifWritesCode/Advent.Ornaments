# Ornaments SDK

An [Advent of Code](https://adventofcode.com) solutions runner and standard library by [Leif Walker-Grant](https://uk.linkedin.com/in/championofgoats).

## (Really) Quick Start

```csharp
// Create a new .NET 7 C# Console App named SomeConsoleApp
// Install Ornaments from NuGet
// Replace Program.cs with this code
// $ dotnet run solve --year 2022 --day 1

using Ornaments.App;
using Ornaments.Solutions;

namespace SomeConsoleApp;

[RegisterOrnament("SomeChallenge", 2022, 1)]
internal sealed class SomeSolution : ISolution
{
    public SomeSolution() {}

    public async Task<object> DoPartOneAsync(ISolutionContext solutionContext)
    {
        return new();
    }

    public async Task<object> DoPartTwoAsync(ISolutionContext solutionContext)
    {
        return new();
    }

    public bool TryParse(string input, out object parsed)
    {
        parsed = new();
        return true;
    }
}

internal class Program
{
    static async Task Main(string[] args)
    {
        await OrnamentApp.CreateDefault().RunAsync(args);
    }
}
```

## Please be Considerate

Ornaments includes a feature enabling automated scraping of, and submission of answers to, the Advent of Code wesbite. Sending a large volume of requests to the Advent of Code website within a short space of time may result in your user-agent and/or IP address being flagged, or you may blocked from making further requests entirely. Please be considerate of Eric, his work, and the many tens of thousands of participants that enjoy the event every year. Please use Ornaments considerably.

A caching mechanism is employed to ensure that Ornaments will not scrape data more than once or, on a per-identity basis, submit the same answer twice. If you wish, you may disable automated submission entirely by setting `global.autosubmit` to `false` in your `appsettings.json`.

## Todo

- [ ] Implement core runner. (In progress)
- [ ] Implement data model.
- [ ] Implement input fetch support.
- [ ] Implement submission support.
- [ ] Implement existing standard library classses.
- [ ] Implement wait support.

## Disclaimer

Ornaments is written by Leif Walker-Grant. Neither Ornaments nor Leif Walker-Grant are affiliated with the Advent of Code in any way.

## Special Mentions

This project exists with my thanks to [Eric Wastl](https://twitter.com/ericwastl) for running the Advent of Code event each year.
