using System.CommandLine.Binding;
using System.CommandLine;

namespace Snowstorm.CommandLine;

internal class SolveCommandOptionsBinder : BinderBase<SolveCommandOptions>
{
    private readonly Option<int> year;
    private readonly Option<int> day;
    private readonly Option<bool> dryRun;
    private readonly Option<int> timeout;

    public SolveCommandOptionsBinder(Option<int> year, Option<int> day, Option<bool> dryRun, Option<int> timeout)
    {
        this.year = year;
        this.day = day;
        this.dryRun = dryRun;
        this.timeout = timeout;
    }

    protected override SolveCommandOptions GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(year),
            bindingContext.ParseResult.GetValueForOption(day),
            bindingContext.ParseResult.GetValueForOption(dryRun),
            bindingContext.ParseResult.GetValueForOption(timeout));
    }
}
