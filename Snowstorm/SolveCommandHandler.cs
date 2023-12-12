using Snowstorm.CommandLine;

namespace Snowstorm;

internal class SolveCommandHandler : ICommandHandler<SolveCommandOptions> 
{
    private readonly IServiceProvider _serviceProvider;

    public SolveCommandHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task HandleAsync(SolveCommandOptions options)
    {
        throw new NotImplementedException();
    }
}