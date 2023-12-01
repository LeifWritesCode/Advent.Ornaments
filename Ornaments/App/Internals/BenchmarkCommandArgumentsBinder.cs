using System.CommandLine;
using System.CommandLine.Binding;

namespace Ornaments.App.Internals;

internal class BenchmarkCommandArgumentsBinder : BinderBase<BenchmarkCommandArguments>
{
    private readonly Option<int> year;
    private readonly Option<int> day;
    private readonly Option<int> runs;

    public BenchmarkCommandArgumentsBinder(Option<int> year, Option<int> day, Option<int> runs)
    {
        this.year = year;
        this.day = day;
        this.runs = runs;
    }

    protected override BenchmarkCommandArguments GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(year),
            bindingContext.ParseResult.GetValueForOption(day),
            bindingContext.ParseResult.GetValueForOption(runs));
    }
}
