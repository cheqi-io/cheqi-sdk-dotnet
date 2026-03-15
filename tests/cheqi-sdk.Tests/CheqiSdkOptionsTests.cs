using Cheqi.Sdk.Config;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;
using Xunit;

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
        var card = new CardDetails(CardProvider.VISA, paymentAccountReference: "PAR123");
        var identification = IdentificationDetails.ForCard(PaymentType.CARD_PAYMENT, card, "customer@example.com");

        Assert.Equal(CardProvider.VISA, card.CardProvider);
        Assert.Equal("PAR123", card.PaymentAccountReference);
        Assert.Equal(PaymentType.CARD_PAYMENT, identification.PaymentType);
        Assert.Equal("customer@example.com", identification.RecipientEmail);
    }
}
