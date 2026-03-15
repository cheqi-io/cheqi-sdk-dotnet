using Cheqi.Sdk.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Cheqi.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="CheqiSdk"/> using idiomatic .NET options configuration.
    /// </summary>
    public static IServiceCollection AddCheqiSdk(
        this IServiceCollection services,
        Action<CheqiSdkOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new CheqiSdkOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton(sp => new CheqiSdk(
            sp.GetRequiredService<CheqiSdkOptions>(),
            sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>(),
            sp.GetRequiredService<CheqiSdkOptions>().HttpClient));

        return services;
    }
}
