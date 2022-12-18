# Ornaments SDK

An [Advent of Code](https://adventofcode.com) solutions runner and standard library by [Leif Walker-Grant](https://uk.linkedin.com/in/championofgoats).

## (Really) Quick Start

A working example project, implementing day 1 of event year 2022, is included. See [ExampleApp](.\ExampleApp), and don't forget to [configure](#configuration) it!

### (Slighty Less) Quick Start

```csharp
// Create a new .NET 7 C# Console App named SomeConsoleApp
// Install Ornaments from NuGet
// Replace Program.cs with this code
// $ dotnet run solve --year 2022 --day 1 --dry-run

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
        await OrnamentsApp.CreateDefault().RunAsync(args);
    }
}
```

## Command Line

Ornaments implements a command line runner capable of listing avaialble solutions, solving challenges, and benchmarking solutions.

To list all challenges --

```
dotnet run list
```

To solve a challenge --

```
dotnet run solve --year 2022 --day 1 [--dry-run] [--users GitHub]
```

Where `--year` is the event year, `--day` is the challenge date, `--dry-run` is an optional flag indicating answers should **not** be submitted, and `--users` specifies the tokens you would like to use.

Lastly, to benchmark a solution --

```
dotnet run benchmark --year 2022 --day 1 [--runs 1000]
```

Where `--year` is the event year, `--day` is the challenge date, and `runs` is an optional integer argument specifying the number of runs to average over.

For more information on how to use each command, and information on the commands themselves including default option values, you can use one of the following --

```
dotnet run -h
dotnet run solve -h
dotnet run list -h
dotnet run benchmark -h
```

## Configuration

In order to comply with Eric's request that all automated submissons carry a unique user-agent, Ornaments requires that you provide a contact email address and fully-qualified URI pointing to the **public** source code of your implementing project. Additionally, if you are not using the `--dry-run` switch, you must specify at least one token.

### Getting a Session Token

Session tokens are obtained by logging in to [Advent of Code](https://adventofcode.com/2022/auth/login) with one of the supported authorisation providers. You can then find the token in the request headers of each subsequent page load. Each token is unique to the provider.

### Minimal Configuration

```json
{
  "tokens": {
    "gitHub": "github_token",
  },
  "ornaments": {
    "emailAddress": "mail@example.com",
    "sourceCodeUri": "https://example.com/user/repo"
  }
}

```

### Full Configuration

```json
{
  "tokens": {
    "gitHub": "github_token",
    "google": "google_token",
    "reddit": "reddit_token",
    "twitter": "twitter_token"
  },
  "ornaments": {
    "database": "database.db",
    "emailAddress": "mail@example.com",
    "sourceCodeUri": "https://example.com/user/repo"
  }
}

```

## Caching

Ornaments records your inputs and submissions in an internal cache. Additionally, after their first use with Ornaments, tokens are recorded in the cache and matched to inputs and submissions. Multiple tokens can be recorded for a given provider, however the token specified in `appsettings.json` at the time of execution shall be used. The cache is stored on your local disk adjacent your project executables.

The singlular copy of your cache exists on your machine, and deleting it is a destructive and permanent operation.  Whilst you can fetch inputs again, you are unable to resubmit answers. Attempting to do so may result in your token(s) being blacklisted by Eric.

## Please be Considerate

Ornaments includes a feature enabling automated scraping of, and submission of answers to, the Advent of Code wesbite. Sending a large volume of requests to the Advent of Code website within a short space of time may result in your user-agent and/or IP address being flagged, or you may blocked from making further requests entirely. Please be considerate of Eric, his work, and the many tens of thousands of participants that enjoy the event every year. Please use Ornaments considerably.

A caching mechanism is employed to ensure that Ornaments will not scrape data more than once or, on a per-identity basis, submit the same answer twice. If you wish, you may disable automated submission entirely by setting `global.autosubmit` to `false` in your `appsettings.json`.

## Todo

- [ ] Implement core runner. (In progress)
- [x] Implement data model.
- [x] Implement input fetch support.
- [x] Implement minimum submission support.
- [ ] Implement existing standard library classses. (In progress)
- [ ] Implement wait support.

## License and Disclaimer

The Ornaments SDK is written by, and the copyright &copy; of, Leif Walker-Grant. Neither Ornaments nor Leif Walker-Grant are affiliated with the Advent of Code in any way. The SDK is made available free-of-charge, as-is and without warranty, under the MIT license. For more information, see LICENSE.

This project is indirectly dependent on SixLabors.ImageSharp, a library which is licensed under Apache 2.0 when disitributed as part of the Ornaments SDK. All other usage is covered by the [Six Labors Split License](https://github.com/SixLabors/ImageSharp/blob/master/LICENSE).

## Special Mentions

This project exists with my thanks to [Eric Wastl](https://twitter.com/ericwastl) for running the Advent of Code event each year.
