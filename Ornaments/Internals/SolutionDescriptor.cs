using Microsoft.Extensions.DependencyInjection;
using Ornaments.Solutions;
using System.Reflection;

namespace Ornaments.Internals;

internal class SolutionDescriptor
{
    public SolutionAttribute Attributes { get; private set; }

    public ISolution Instance { get; private set; }

    public SolutionDescriptor(IServiceProvider serviceProvider, Type type)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        if (!type.IsAssignableTo(typeof(ISolution)))
            throw new InvalidCastException($"Specified type is not a solution.");

        var attribute = type.GetCustomAttribute<SolutionAttribute>();
        if (attribute is null)
            throw new ArgumentException("Solution must have a SolutionAttribute.");

        Attributes = attribute;
        Instance = (ISolution)ActivatorUtilities.CreateInstance(serviceProvider, type);
        if (Instance is null)
            throw new ArgumentException("Unable to create an instance of the given type.");
    }
}
