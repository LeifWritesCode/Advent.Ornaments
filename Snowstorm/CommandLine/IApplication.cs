namespace Snowstorm.CommandLine;

public interface IApplication
{
    Task StartAsync(string[] args);
}
