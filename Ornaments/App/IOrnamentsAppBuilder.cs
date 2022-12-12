using Microsoft.Extensions.DependencyInjection;

namespace Ornaments.App;

public interface IOrnamentsAppBuilder
{
    IOrnamentsAppBuilder ConfigureServices(Action<IServiceCollection> configure);

    IOrnamentsApp Build(); 
}
