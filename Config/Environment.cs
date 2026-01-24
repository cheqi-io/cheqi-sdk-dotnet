namespace Cheqi.Sdk.Config;

/// <summary>
/// Predefined API environments for the Cheqi SDK.
/// </summary>
public enum Environment
{
    /// <summary>
    /// Sandbox environment for development and testing.
    /// </summary>
    Sandbox,

    /// <summary>
    /// Test environment for integration testing.
    /// </summary>
    Test,

    /// <summary>
    /// Production environment for live transactions.
    /// </summary>
    Production
}

/// <summary>
/// Extension methods for the Environment enum.
/// </summary>
public static class EnvironmentExtensions
{
    /// <summary>
    /// Gets the base URL for the specified environment.
    /// </summary>
    public static string GetBaseUrl(this Environment environment) => environment switch
    {
        Environment.Sandbox => "https://sandbox.api.cheqi.io",
        Environment.Test => "https://test.api.cheqi.io",
        Environment.Production => "https://api.cheqi.io",
        _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unknown environment")
    };
}
