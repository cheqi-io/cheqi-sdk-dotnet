namespace Cheqi.Sdk.Config;

/// <summary>
/// Predefined Cheqi API environments.
/// 
/// Use these for standard deployments, or use customApiEndpoint() for custom URLs.
/// </summary>
public enum Environment
{
    /// <summary>
    /// Sandbox environment for testing and development.
    /// </summary>
    SANDBOX,

    /// <summary>
    /// Test environment for integration testing.
    /// </summary>
    TEST,

    /// <summary>
    /// Production environment for live operations.
    /// </summary>
    PRODUCTION
}

/// <summary>
/// Extension methods for the Environment enum.
/// </summary>
public static class EnvironmentExtensions
{
    /// <summary>
    /// Returns the base URL for this environment.
    /// </summary>
    /// <param name="environment">The environment</param>
    /// <returns>The API base URL</returns>
    public static string GetBaseUrl(this Environment environment) => environment switch
    {
        Environment.SANDBOX => "https://sandbox.api.cheqi.io",
        Environment.TEST => "https://test.api.cheqi.io",
        Environment.PRODUCTION => "https://api.cheqi.io",
        _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unknown environment")
    };
}
