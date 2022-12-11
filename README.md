# Ornaments SDK
An [Advent of Code](https://adventofcode.com) solutions runner and standard library.

## How to Use this SDK

1. Add reference to Ornaments to your project.
2. In your main method, add...

```csharp
await OrnamentApp.CreateDefault().RunAsync(args);
```

3. Add a new class implementing `ISolution`.
4. Decorate (3) with a `RegisterOrnamentAttribute` describing name, year, and date.
5. Build and run by issuing `dotnet run solve -y <year> -d <day>` from the root of your project.

## Get Started Fast Copypasta

```csharp
// Program.cs

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

## Todo

- [ ] Implement core runner. (In progress)
- [ ] Implement data model.
- [ ] Implement input fetch support.
- [ ] Implement submission support.
- [ ] Implement existing standard library classses.
- [ ] Implement wait support.

<sub>With thanks to [Eric Wastl](https://twitter.com/ericwastl) for running this wonderful event each year.</sub>
