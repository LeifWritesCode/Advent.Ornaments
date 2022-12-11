using System.CommandLine;
using System.CommandLine.Binding;

namespace Ornaments.Internals;

internal class AdventAppOptionsBinder : BinderBase<AdventAppOptions>
{
    public AdventAppOptionsBinder(Option<int> year, Option<int> day, Option<bool> dryRun)
    {

    }

    protected override AdventAppOptions GetBoundValue(BindingContext bindingContext)
    {
        return new();
    }
}
