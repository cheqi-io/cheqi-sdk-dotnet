namespace Cheqi.Sdk.Config;

/// <summary>
/// Idiomatic .NET API environment enum for the options-based SDK API.
/// </summary>
public enum CheqiApiEnvironment
{
    Sandbox,
    Test,
    Production
}

public static class CheqiApiEnvironmentExtensions
{
    public static string GetBaseUrl(this CheqiApiEnvironment environment) => environment switch
    {
        CheqiApiEnvironment.Sandbox => "https://sandbox.api.cheqi.io",
        CheqiApiEnvironment.Test => "https://test.api.cheqi.io",
        CheqiApiEnvironment.Production => "https://api.cheqi.io",
        _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unknown environment")
    };
}
