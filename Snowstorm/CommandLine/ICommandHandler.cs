namespace Snowstorm.CommandLine;

internal interface ICommandHandler<Toptions>
{
    Task HandleAsync(Toptions options);
}
