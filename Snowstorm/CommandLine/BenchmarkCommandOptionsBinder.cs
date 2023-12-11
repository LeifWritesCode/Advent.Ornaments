using System.CommandLine.Binding;
using System.CommandLine;

namespace Snowstorm.CommandLine;

internal class BenchmarkCommandOptionsBinder : BinderBase<BenchmarkCommandOptions>
{
    private readonly Option<int> year;
    private readonly Option<int> day;
    private readonly Option<int> runs;

    public BenchmarkCommandOptionsBinder(Option<int> year, Option<int> day, Option<int> runs)
    {
        this.year = year;
        this.day = day;
        this.runs = runs;
    }

    protected override BenchmarkCommandOptions GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(year),
            bindingContext.ParseResult.GetValueForOption(day),
            bindingContext.ParseResult.GetValueForOption(runs));
    }
}
