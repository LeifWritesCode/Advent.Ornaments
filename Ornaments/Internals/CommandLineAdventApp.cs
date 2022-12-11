using Microsoft.Extensions.DependencyInjection;
using Ornaments.App;
using Ornaments.Extensions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace Ornaments.Internals;

internal class CommandLineAdventApp : IOrnamentApp
{
    private readonly IServiceProvider serviceProvider;
    private readonly RootCommand rootCommand;

    public CommandLineAdventApp(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        var year = new Option<int>(new[] { "-y", "--year" }, () => 2022, "Advent of Code event year.");
        var day = new Option<int>(new[] { "-d", "--day" }, () => 1, "Advent of Code event date.");
        var dryRun = new Option<bool>("--dry-run", () => false, "Disable automatic submission of answers.");
        var tokenTypes = new Option<IEnumerable<TokenType>>(new[] { "-u", "--users" }, () => new[] { TokenType.GitHub }, "Specifies which user tokens to submit answers for.");

        year.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            if (value.IsNotInRange(2015, DateTime.Now.Year))
            {
                result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
            }
        });
        day.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<int>();
            if (value.IsNotInRange(1, 25))
            {
                result.ErrorMessage = "Value must be between 1 and 25 (inclusive.)";
            }
        });

        var solveCommand = new Command("solve", "Run Advent of Code event solutions.") { year, day, dryRun, tokenTypes };
        var adventOptionsBinder = new OrnamentAppOptionsBinder(year, day, dryRun);
        solveCommand.SetHandler(HandleSolveCommandAsync, adventOptionsBinder);

        var includeYears = new Option<IEnumerable<int>>(new[] { "-y", "--year" }, () => Enumerable.Range(2015, DateTime.Now.Year - 2015 + 1), "Advent of Code event year.");

        includeYears.AddValidator(result =>
        {
            var value = result.GetValueOrDefault<IEnumerable<int>>() ?? Enumerable.Empty<int>();
            if (value.Any(x => x.IsNotInRange(2015, DateTime.Now.Year)))
            {
                result.ErrorMessage = $"Value must be between 2015 and {DateTime.Now.Year} (inclusive.)";
            }
        });

        var listCommand = new Command("list", "List available Advent of Code event solutions.") { includeYears };
        listCommand.SetHandler(HandleListCommandAsync, includeYears);

        rootCommand = new RootCommand("Ornament — An Advent of Code SDK.");
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(solveCommand);
    }

    public async Task RunAsync(string[] args)
    {
        await rootCommand.InvokeAsync(args);
    }

    private async Task HandleSolveCommandAsync(OrnamentAppOptions adventAppOptions)
    {
        var solutionDescriptors = serviceProvider.GetRequiredService<IEnumerable<SolutionDescriptor>>();
        if (solutionDescriptors.IsEmpty())
        {
            Console.WriteLine("There are no solutions registered.");
            return;
        }

        var descriptor = solutionDescriptors.SingleOrDefault(x => x.Attributes.Year == adventAppOptions.Year && x.Attributes.Day == adventAppOptions.Day);
        if (descriptor is null)
        {
            Console.WriteLine($"There are no solutions registered matching event year {adventAppOptions.Year}, day {adventAppOptions.Day}.");
            return;
        }

        Console.WriteLine($"Day {adventAppOptions.Day}, {adventAppOptions.Year}: {descriptor.Attributes.Name}");
        await descriptor.Instance.RunAsync();
    }

    private void HandleListCommandAsync(IEnumerable<int> years)
    {
        var solutionDescriptors = serviceProvider.GetRequiredService<IEnumerable<SolutionDescriptor>>();
        if (solutionDescriptors.IsEmpty())
        {
            Console.WriteLine("There are no solutions registered.");
            return;
        }

        foreach (var year in years)
        {
            Console.WriteLine($"Solutions for event year {year}:");
            var descriptorsForYear = solutionDescriptors.Where(x => x.Attributes.Year == year);
            if (descriptorsForYear.IsEmpty())
            {
                Console.WriteLine("\tNothing to show. :(");
            }
            else
            {
                foreach (var descriptor in descriptorsForYear)
                {
                    Console.WriteLine($"\tDay {descriptor.Attributes.Day}: {descriptor.Attributes.Name}.");
                }
            }
            Console.WriteLine();
        }
    }
}
