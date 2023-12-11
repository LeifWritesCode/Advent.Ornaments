using System.CommandLine.Binding;
using System.CommandLine;

namespace Snowstorm.CommandLine;

internal class ListCommandOptionsBinder : BinderBase<ListCommandOptions>
{
    private readonly Option<IEnumerable<int>> years;

    public ListCommandOptionsBinder(Option<IEnumerable<int>> years)
    {
        this.years = years;
    }

    protected override ListCommandOptions GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(years) ?? Enumerable.Empty<int>());
    }
}
