using Microsoft.Extensions.DependencyInjection;

namespace Ornaments.App;

public interface IOrnamentAppBuilder
{
    IOrnamentAppBuilder ConfigureServices(Action<IServiceCollection> configure);

    IOrnamentApp Build(); 
}
