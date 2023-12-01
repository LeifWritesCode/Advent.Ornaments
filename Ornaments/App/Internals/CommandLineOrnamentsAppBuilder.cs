using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ornaments.Data;
using Ornaments.Extensions;
using Ornaments.App.Internals.Net;
using Ornaments.Solutions;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace Ornaments.App.Internals;

internal class CommandLineOrnamentsAppBuilder : IOrnamentsAppBuilder
{
    private readonly IServiceCollection serviceCollection;
    private readonly Queue<Action<IServiceCollection>> configurations;

    public CommandLineOrnamentsAppBuilder()
    {
        serviceCollection = new ServiceCollection();
        configurations = new Queue<Action<IServiceCollection>>();
    }

    public IOrnamentsAppBuilder ConfigureServices(Action<IServiceCollection> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configurations.Enqueue(configuration);
        return this;
    }

    public IOrnamentsApp Build()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();

        // add an http client for each token
        var tokenTypes = Enum.GetValues<TokenType>();
        var tokenOptions = new TokenOptions();
        configuration.GetSection(TokenOptions.Section).Bind(tokenOptions);
        foreach (var tokenType in tokenTypes)
        {
            if (tokenOptions.TryGet(tokenType, out var token))
            {
                AddHttpClient(serviceCollection, configuration, tokenType.ToString(), token);
            }
        }

        // and one more (sans session token) for the aoc server
        serviceCollection.AddTransient<AocRestClient>();
        AddHttpClient(serviceCollection, configuration, nameof(AocRestClient));

        var solutions = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(ISolution)))
            .Where(x => x.IsClass)
            .Where(x => !x.IsAbstract)
            .Where(x => x.GetCustomAttribute<RegisterOrnamentAttribute>() is not null);

        foreach (var solution in solutions)
            serviceCollection.AddTransient(sp => new SolutionDescriptor(sp, solution));

        serviceCollection.AddOptions<TokenOptions>()
            .Configure(tokenOptions => configuration.GetSection(TokenOptions.Section).Bind(tokenOptions));
        serviceCollection.AddOptions<OrnamentsOptions>()
            .Configure(ornamentOptions => configuration.GetSection(OrnamentsOptions.Section).Bind(ornamentOptions));

        serviceCollection.AddDbContext<OrnamentsContext>();
        serviceCollection.AddTransient(typeof(ILogger<>), typeof(SpectreLogger<>));

        while (configurations.Any())
            configurations.Dequeue().Invoke(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        return new CommandLineOrnamentsApp(serviceProvider);
    }

    private static void AddHttpClient(IServiceCollection serviceCollection, IConfiguration configuration, string name, string token = "")
    {
        var httpClientBuilder = serviceCollection.AddHttpClient(name);
        httpClientBuilder.ConfigureHttpClient(config =>
        {
            var assemblyFileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            // var name = assemblyFileVersionInfo.ProductName ?? assemblyFileVersionInfo.FileName;
            var version = assemblyFileVersionInfo.ProductVersion;
            var ornamentOptions = new OrnamentsOptions();
            configuration.GetSection(OrnamentsOptions.Section).Bind(ornamentOptions);

            ArgumentException.ThrowIfNullOrEmpty(ornamentOptions.EmailAddress);
            ArgumentException.ThrowIfNullOrEmpty(ornamentOptions.SourceCodeUri);

            var compatibleValue = new ProductInfoHeaderValue("Mozilla", "5.50");
            var ornamentCommentValue = new ProductInfoHeaderValue("(compatible; Ornaments SDK; contact hei@lwg.no; source https://github.com/leifwritescode/ornaments;)");
            var productValue = new ProductInfoHeaderValue("Ornaments", version);
            var productCommentValue = new ProductInfoHeaderValue($"(contact {ornamentOptions.EmailAddress}; source {ornamentOptions.SourceCodeUri};)");

            config.DefaultRequestHeaders.UserAgent.Add(compatibleValue);
            config.DefaultRequestHeaders.UserAgent.Add(ornamentCommentValue);
            config.DefaultRequestHeaders.UserAgent.Add(productValue);
            config.DefaultRequestHeaders.UserAgent.Add(productCommentValue);
            config.BaseAddress = new Uri("https://adventofcode.com");
        });

        if (token.IsEmpty())
            return;

        var sessionCookie = new Cookie("session", token)
        {
            Domain = "adventofcode.com"
        };
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(sessionCookie);
        httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        {
            CookieContainer = cookieContainer
        });
    }
}
