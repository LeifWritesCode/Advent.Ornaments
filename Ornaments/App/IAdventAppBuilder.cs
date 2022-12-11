using Microsoft.Extensions.DependencyInjection;

namespace Ornaments.App;

public interface IAdventAppBuilder
{
    IAdventAppBuilder ConfigureServices(Action<IServiceCollection> configure);

    IAdventApp Build(); 
}
