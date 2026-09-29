using Cheqi.Sdk.Config;
using Cheqi.Sdk.Tests.TestSupport;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Cheqi.Sdk.Extensions;

namespace Cheqi.Sdk.Tests;

public sealed class CheqiSdkOptionsTests
{
    [Fact]
    public void CanCreateSdkFromOptions()
    {
        using var sdk = Cheqi.Sdk.CheqiSdk.Create(new CheqiSdkOptions
        {
            Environment = CheqiApiEnvironment.Sandbox,
            ApiKey = "sk_test_example",
            TimeoutSeconds = 45,
            MaxRetries = 5
        });

        Assert.Equal("https://sandbox.api.cheqi.io", sdk.Config.ApiEndpoint);
        Assert.Equal("sk_test_example", sdk.Config.ApiKey);
        Assert.Equal(45, sdk.Config.TimeoutSeconds);
        Assert.Equal(5, sdk.Config.MaxRetries);
        Assert.NotNull(sdk.VerificationService);
    }

    [Fact]
    public void CommonModelsCanBeCreatedWithoutBuilders()
    {
        var card = new Cheqi.Sdk.Models.Generated.CardDetails("PAR123");
        card.CardProvider = Cheqi.Sdk.Models.Generated.CardDetails.CardProviderEnum.VISA;

        var identification = new Cheqi.Sdk.Models.Generated.IdentificationDetails(
            Cheqi.Sdk.Models.Generated.PaymentType.CARDPAYMENT);
        identification.CardDetails = card;
        identification.RecipientEmail = "customer@example.com";

        Assert.Equal(Cheqi.Sdk.Models.Generated.CardDetails.CardProviderEnum.VISA, card.CardProvider);
        Assert.Equal("PAR123", card.PaymentAccountReference);
        Assert.Equal(Cheqi.Sdk.Models.Generated.PaymentType.CARDPAYMENT, identification.PaymentType);
        Assert.Equal("customer@example.com", identification.RecipientEmail);
    }

    [Fact]
    public void InvalidApiKeyFormat_DoesNotEmitSdkWarning()
    {
        var loggerProvider = new ListLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));

        _ = CheqiSdkConfig.FromOptions(new CheqiSdkOptions
        {
            Environment = CheqiApiEnvironment.Sandbox,
            ApiKey = "nonstandard-key"
        }, loggerFactory.CreateLogger<CheqiSdkConfig>());

        var combinedLogs = string.Join("\n", loggerProvider.Messages);
        Assert.DoesNotContain("API key does not match expected format", combinedLogs);
    }
    [Fact]
    public void DependencyInjectionCallbackCanConfigureOptionsAndCreatesSingleton()
    {
        var services = new ServiceCollection();
        services.AddCheqiSdk(options =>
        {
            options.Environment = CheqiApiEnvironment.Test;
            options.ApiKey = "configured";
        });
        using var provider = services.BuildServiceProvider();
        var sdk = provider.GetRequiredService<CheqiSdk>();
        Assert.Same(sdk, provider.GetRequiredService<CheqiSdk>());
        Assert.Equal("configured", sdk.Config.ApiKey);
        Assert.Equal("https://test.receipt.cheqi.io", sdk.Config.ReceiptDownloadBaseUrl);
    }

    [Theory]
    [InlineData(CheqiApiEnvironment.Production, "https://receipt.cheqi.io")]
    [InlineData(CheqiApiEnvironment.Sandbox, "https://sandbox.receipt.cheqi.io")]
    [InlineData(CheqiApiEnvironment.Test, "https://test.receipt.cheqi.io")]
    public void StandardApiEnvironmentsSelectReceiptFrontend(CheqiApiEnvironment environment, string expected)
        => Assert.Equal(expected, CheqiSdkConfig.FromOptions(new CheqiSdkOptions { Environment = environment }).ReceiptDownloadBaseUrl);

    [Fact]
    public void CustomApiDoesNotGuessDownloadOrigin()
    {
        Assert.Null(CheqiSdkConfig.FromOptions(new CheqiSdkOptions { CustomApiEndpoint = "http://localhost:8080" }).ReceiptDownloadBaseUrl);
        Assert.Equal("http://localhost:3000", CheqiSdkConfig.FromOptions(new CheqiSdkOptions
        { CustomApiEndpoint = "http://localhost:8080", ReceiptDownloadBaseUrl = "http://localhost:3000" }).ReceiptDownloadBaseUrl);
    }
}
