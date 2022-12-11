using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ornaments.App;
using Ornaments.Solutions;
using System.Reflection;

namespace Ornaments.Internals;

internal class CommandLineAdventAppBuilder : IOrnamentAppBuilder
{
    private readonly IServiceCollection serviceCollection;
    private readonly Queue<Action<IServiceCollection>> configurations;

    public CommandLineAdventAppBuilder()
    {
        serviceCollection = new ServiceCollection();
        configurations = new Queue<Action<IServiceCollection>>();
    }

    public IOrnamentAppBuilder ConfigureServices(Action<IServiceCollection> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configurations.Enqueue(configuration);
        return this;
    }

    public IOrnamentApp Build()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();

        var solutions = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(ISolution)))
            .Where(x => x.IsClass)
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetCustomAttribute<RegisterOrnamentAttribute>() is not null);

        foreach (var solution in solutions )
            serviceCollection.AddTransient(sp => new SolutionDescriptor(sp, solution));

        while (configurations.Any())
            configurations.Dequeue().Invoke(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        return new CommandLineAdventApp(serviceProvider);
    }
}
