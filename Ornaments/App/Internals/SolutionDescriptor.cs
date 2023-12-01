using Microsoft.Extensions.DependencyInjection;
using Ornaments.Solutions;
using System.Reflection;

namespace Ornaments.App.Internals;

internal class SolutionDescriptor
{
    public RegisterOrnamentAttribute Attributes { get; private set; }

    public ISolution Instance { get; private set; }

    public SolutionDescriptor(IServiceProvider serviceProvider, Type type)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        if (!type.IsAssignableTo(typeof(ISolution)))
            throw new InvalidCastException($"Specified type is not an implementation of {nameof(ISolution)}.");

        var attribute = type.GetCustomAttribute<RegisterOrnamentAttribute>();
        if (attribute is null)
            throw new ArgumentException($"Solution must have a {nameof(RegisterOrnamentAttribute)}.");

        Attributes = attribute;
        Instance = (ISolution)ActivatorUtilities.CreateInstance(serviceProvider, type);
        if (Instance is null)
            throw new ArgumentException("Unable to create an instance of the given type.");
    }
}
