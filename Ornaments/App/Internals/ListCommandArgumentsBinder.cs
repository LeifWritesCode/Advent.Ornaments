using System.CommandLine;
using System.CommandLine.Binding;

namespace Ornaments.App.Internals;

internal class ListCommandArgumentsBinder : BinderBase<ListCommandArguments>
{
    private readonly Option<IEnumerable<int>> years;

    public ListCommandArgumentsBinder(Option<IEnumerable<int>> years)
    {
        this.years = years;
    }

    protected override ListCommandArguments GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(years) ?? Enumerable.Empty<int>());
    }
}
