using System.CommandLine;
using System.CommandLine.Binding;

namespace Ornaments.Internals;

internal class OrnamentAppOptionsBinder : BinderBase<OrnamentAppOptions>
{
    private readonly Option<int> year;
    private readonly Option<int> day;
    private readonly Option<bool> dryRun;

    public OrnamentAppOptionsBinder(Option<int> year, Option<int> day, Option<bool> dryRun)
    {
        this.year = year;
        this.day = day;
        this.dryRun = dryRun;
    }

    protected override OrnamentAppOptions GetBoundValue(BindingContext bindingContext)
    {
        return new()
        {
            Year = bindingContext.ParseResult.GetValueForOption(year),
            Day = bindingContext.ParseResult.GetValueForOption(day),
            DryRun = bindingContext.ParseResult.GetValueForOption(dryRun)
        };
    }
}
