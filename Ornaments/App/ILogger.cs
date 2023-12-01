namespace Ornaments.App;

public interface ILogger<TClass> where TClass : class
{
    void Verbose(string message, params object[] args);

    void Information(string message, params object[] args);

    void Warning(string message, params object[] args);

    void Error(string message, params object[] args);
}
