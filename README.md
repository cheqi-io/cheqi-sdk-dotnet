# Cheqi .NET SDK

[![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

The Cheqi .NET SDK provides end-to-end encrypted receipt and credit note processing capabilities for suppliers, enabling secure digital receipt delivery to customers through the Cheqi platform.

## Features

- **End-to-End Encryption**: Hybrid RSA+AES encryption for secure receipt delivery
- **Customer Matching**: Match customers using payment identifiers (cards, IBANs, email)
- **Receipt Processing**: Create and deliver encrypted receipts
- **Credit Note Processing**: Handle returns and refunds with encryption
- **RFC8785 Canonical JSON**: Deterministic JSON canonicalization for hash consistency
- **Cross-Platform Hash Verification**: Consistent hashing across .NET, Java, and iOS
- **Company Provisioning**: Provision companies and receive API credentials
- **Store Management**: Full CRUD operations for store/location management
- **Multi-Environment Support**: Sandbox, Test, and Production environments
- **Automatic Retry Logic**: Built-in retry with exponential backoff
- **Async/Await**: Modern asynchronous programming throughout

## Installation

### NuGet Package Manager

```bash
Install-Package cheqi-sdk -Version 1.0.0
```

### .NET CLI

```bash
dotnet add package cheqi-sdk --version 1.0.0
```

### PackageReference

Add to your `.csproj` file:

```xml
<PackageReference Include="cheqi-sdk" Version="1.0.0" />
```

## Quick Start

```csharp
using Cheqi.Sdk;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;

// 1. Initialize the SDK
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Sandbox)
    .ApiKey("your-api-key")
    .Build();

// 2. Create customer identification details
var customer = new IdentificationDetails
{
    CardDetails = new CardDetails
    {
        Par = "PAR123456789",
        CardProvider = CardProvider.VISA
    }
};

// 3. Build the receipt request
var receipt = new ReceiptTemplateRequest
{
    ReceiptId = "INV-001",
    TransactionDateTime = DateTimeOffset.UtcNow,
    CurrencyCode = "EUR",
    TotalAmount = 12.10m,
    TotalTaxAmount = 2.10m,
    PaymentType = PaymentType.CARD_PAYMENT,
    Products = new List<Product>
    {
        new Product
        {
            Name = "Coffee",
            Quantity = 1,
            UnitCode = UnitCode.EA,
            UnitPrice = 10.00m,
            Tax = new Tax { Percent = 21.0m, TaxAmount = 2.10m }
        }
    }
};

// 4. Process the receipt (matches customer, encrypts, and delivers)
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(customer, receipt);

if (result.IsSuccess)
{
    Console.WriteLine($"Receipt delivered to {result.ReceiptCount} devices");
}
else if (!result.RecipientsFound)
{
    Console.WriteLine("No Cheqi users found for this customer");
}
else
{
    Console.WriteLine($"Failed: {result.ErrorMessage}");
}
```

## Environment Configuration

| Environment | URL | Description |
|-------------|-----|-------------|
| `Environment.Sandbox` | `https://sandbox.api.cheqi.io` | Development |
| `Environment.Test` | `https://test.api.cheqi.io` | Testing |
| `Environment.Production` | `https://api.cheqi.io` | Production |

## Authentication

The SDK supports two authentication methods:

### API Key

```csharp
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("sk_live_your_api_key")
    .Build();
```

### OAuth Client Credentials

```csharp
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .SupplierCredentials("client-id", "client-secret")
    .Build();
```

## Configuration Options

```csharp
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("your-api-key")
    .Timeout(TimeSpan.FromSeconds(60))
    .MaxRetries(5)
    .Build();
```

## Customer Matching

Match customers using various payment identifiers:

```csharp
// By card details
var customer = new IdentificationDetails
{
    CardDetails = new CardDetails
    {
        Par = "PAR123456789",
        CardProvider = CardProvider.VISA
    }
};

// By IBAN
var customer = new IdentificationDetails
{
    PaymentAccountDetails = new PaymentAccountDetails
    {
        Iban = "NL91ABNA0417164300"
    }
};

// By email
var customer = new IdentificationDetails
{
    Email = "customer@example.com"
};

// By Cheqi Receipt ID (for credit notes)
var customer = new IdentificationDetails
{
    CheqiReceiptId = "original-receipt-id"
};

// Match directly
var matchResult = await sdk.MatchingService.MatchCustomerAsync(customer);
if (matchResult.HasRecipients)
{
    Console.WriteLine($"Found {matchResult.Recipients.Count} devices");
}
```

## Receipt Processing

### Complete Receipt Flow

The `ProcessCompleteReceiptAsync` method handles the entire flow:

1. Matches the customer using identification details
2. Generates a receipt template
3. Encrypts the receipt for each recipient device
4. Delivers the encrypted receipts

```csharp
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
    identificationDetails,
    receiptRequest
);

// With explicit access token
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
    identificationDetails,
    receiptRequest,
    accessToken
);
```

### Building Products

```csharp
var product = new Product
{
    Name = "MacBook Pro 14\"",
    Description = "Apple M3 Pro chip",
    Sku = "MBP-14-2024",
    Ean = "0195949185441",
    Quantity = 1,
    UnitCode = UnitCode.EA,
    UnitPrice = 2499.00m,
    LineTotal = 2499.00m,
    LineTotalIncludingTax = 3023.79m,
    Tax = new Tax
    {
        CategoryCode = "S",
        Percent = 21.0m,
        TaxAmount = 524.79m,
        TaxableAmount = 2499.00m
    },
    DiscountAmount = 100.00m,
    Category = "Electronics"
};
```

### Receipt Request

```csharp
var receipt = new ReceiptTemplateRequest
{
    ReceiptId = "INV-2024-001",
    TransactionDateTime = DateTimeOffset.UtcNow,
    CurrencyCode = "EUR",
    TotalAmount = 121.00m,
    TotalAmountExcludingTax = 100.00m,
    TotalTaxAmount = 21.00m,
    PaymentType = PaymentType.CARD_PAYMENT,
    StoreId = "store-001",
    StoreName = "Amsterdam Central",
    StoreAddress = "Kalverstraat 1, Amsterdam",
    TerminalId = "POS-001",
    CashierId = "employee-123",
    Products = products,
    Taxes = new List<Tax>
    {
        new Tax { CategoryCode = "S", Percent = 21.0m, TaxAmount = 21.00m, TaxableAmount = 100.00m }
    },
    Notes = "Thank you for your purchase!"
};
```

## Credit Note Processing

Process returns and refunds with the same encryption:

```csharp
using Cheqi.Sdk.CreditNote;

var creditNote = new CreditNoteTemplateRequest
{
    CreditNoteId = "CN-2024-001",
    OriginalReceiptId = "INV-2024-001",
    IssueDateTime = DateTimeOffset.UtcNow,
    CurrencyCode = "EUR",
    TotalAmount = 121.00m,
    TotalTaxAmount = 21.00m,
    Reason = "Product return - defective item",
    Products = returnedProducts
};

var result = await sdk.CreditNoteService.ProcessCompleteCreditNoteAsync(
    identificationDetails,
    creditNote
);
```

## Company Provisioning

Provision new companies (requires partner credentials):

```csharp
using Cheqi.Sdk.Models.Company;

var company = new Company
{
    Name = "My Store BV",
    TradingName = "My Store",
    RegistrationNumber = "12345678",
    VatNumber = "NL123456789B01",
    Email = "info@mystore.nl",
    Address = new Address
    {
        StreetAddress = "Hoofdstraat 123",
        City = "Amsterdam",
        PostalCode = "1012AB",
        CountryCode = "NL"
    }
};

var response = await sdk.CompanyService.ProvisionCompanyAsync(
    new ProvisionCompanyRequest
    {
        Company = company,
        AdminEmail = "admin@mystore.nl"
    }
);

if (response.Success)
{
    Console.WriteLine($"Company ID: {response.Company?.Id}");
    Console.WriteLine($"API Key: {response.ApiKey}");
}
```

## Store Management

Manage stores for a company:

```csharp
// Create a store
var store = await sdk.StoreService.CreateStoreAsync(
    companyId,
    new CreateStoreRequest
    {
        Name = "Amsterdam Central",
        StoreCode = "AMS-001",
        Email = "amsterdam@mystore.nl",
        Phone = "+31201234567",
        Address = new Address
        {
            StreetAddress = "Kalverstraat 1",
            City = "Amsterdam",
            PostalCode = "1012NX",
            CountryCode = "NL"
        }
    }
);

// List all stores
var stores = await sdk.StoreService.GetStoresAsync(companyId);

// Get a specific store
var store = await sdk.StoreService.GetStoreAsync(companyId, storeId);

// Update a store
var updated = await sdk.StoreService.UpdateStoreAsync(companyId, storeId, updateRequest);

// Activate/Deactivate
await sdk.StoreService.ActivateStoreAsync(companyId, storeId);
await sdk.StoreService.DeactivateStoreAsync(companyId, storeId);

// Delete a store
await sdk.StoreService.DeleteStoreAsync(companyId, storeId);
```

## Direct Service Access

Access individual services for advanced operations:

```csharp
var encryption = sdk.EncryptionService;
var decryption = sdk.DecryptionService;
var matching = sdk.MatchingService;
var receipts = sdk.ReceiptService;
var creditNotes = sdk.CreditNoteService;
var companies = sdk.CompanyService;
var stores = sdk.StoreService;
```

## Error Handling

```csharp
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;

try
{
    var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(customer, receipt);
}
catch (CheqiApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseBody}");
}
catch (ReceiptProcessingException ex)
{
    Console.WriteLine($"Receipt Error: {ex.Message}");
}
catch (MatchingException ex)
{
    Console.WriteLine($"Matching Error: {ex.Message}");
}
catch (CheqiSdkException ex)
{
    Console.WriteLine($"SDK Error: {ex.Message}");
}
```

## Security

### Encryption

- **Hybrid Encryption**: RSA-OAEP for key exchange + AES-256-GCM for data
- **Forward Secrecy**: Unique AES key generated for each receipt
- **Integrity Protection**: GCM authentication tags
- **Multi-Device Support**: Encrypt once per recipient device

### Receipt Integrity & Hash Verification

The SDK implements RFC8785 canonical JSON to ensure consistent hash generation across all platforms:

- **Deterministic JSON**: Receipt data is canonicalized using RFC8785 standard
- **Cross-Platform Consistency**: Identical hashes generated by .NET, Java, and iOS SDKs
- **Tamper Detection**: Any modification to receipt data changes the hash
- **Third-Party Verification**: External parties can verify receipt authenticity

```csharp
// The SDK automatically handles canonicalization and hashing
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(customer, receipt);

// Hash is calculated from RFC8785 canonical JSON
// This ensures the same receipt produces identical hashes across platforms
```

**Key Implementation Details:**

1. **Compact JSON**: All JSON is serialized without whitespace or indentation
2. **Null Exclusion**: Backend configured to exclude null fields from responses
3. **Invariant Culture**: Decimal numbers use dot (`.`) as decimal separator
4. **Original JSON Preservation**: Receipt template from backend is preserved to avoid re-serialization differences
5. **Direct JSON Manipulation**: Payment means are injected directly into JSON without full deserialization

### Decryption (for receiving encrypted data)

```csharp
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("your-api-key")
    .PrivateKey(privateKeyPem)  // Your RSA private key
    .Build();

// Decrypt received data
var decryptedJson = sdk.DecryptionService.DecryptReceipt(encryptedData);
```

## Technical Details

### JSON Serialization

The SDK uses `System.Text.Json` with custom configuration for RFC8785 compliance:

```csharp
var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    WriteIndented = false, // Compact JSON for canonicalization
    Converters = { new DecimalJsonConverter() } // Culture-invariant decimals
};
```

### Decimal Handling

Custom `DecimalJsonConverter` ensures consistent number formatting:

- Uses `CultureInfo.InvariantCulture` for all decimal serialization
- Prevents locale-specific formatting (e.g., comma as decimal separator)
- Strips trailing zeros while preserving precision
- Ensures JSON-compatible output across all cultures

### Canonical JSON Process

1. **Receipt Template**: Fetched from backend as compact JSON
2. **Payment Means Injection**: Added using `JsonDocument.Parse` and `Utf8JsonWriter`
3. **Canonicalization**: RFC8785 standard applied (key sorting, number normalization)
4. **Hash Generation**: SHA-256 hash of canonical JSON
5. **Verification**: Hash submitted to backend for integrity tracking

## Requirements

- **.NET**: 6.0 or higher
- **Dependencies**: Managed via NuGet
  - `Microsoft.Extensions.Logging.Abstractions`
  - `Microsoft.Extensions.Http`

## Building from Source

```bash
git clone https://github.com/cheqi/cheqi-sdk-dotnet.git
cd cheqi-sdk-dotnet

dotnet restore
dotnet build
dotnet test
dotnet pack
```

## Email Delivery

When customers don't have the Cheqi app, receipts can be sent via email:

```csharp
// Email delivery with API key
var result = await sdk.ReceiptService.SendReceiptViaEmailAsync(
    "customer@example.com",
    purchaseReceipt
);

// Email delivery with OAuth token
var result = await sdk.ReceiptService.SendReceiptViaEmailAsync(
    "customer@example.com",
    purchaseReceipt,
    accessToken
);

if (result.DeliveryMethod == DeliveryMethod.Email)
{
    Console.WriteLine($"Receipt sent to {result.EmailAddress}");
}
```

## Advanced Usage Patterns

### Dependency Injection

Integrate the SDK with ASP.NET Core DI:

```csharp
// Startup.cs or Program.cs
services.AddSingleton<CheqiSdk>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    
    return CheqiSdk.Builder()
        .ApiEndpoint(Environment.Production)
        .ApiKey(config["Cheqi:ApiKey"])
        .TimeoutSeconds(60)
        .MaxRetries(3)
        .Build();
});

// Use in controllers
public class ReceiptController : ControllerBase
{
    private readonly CheqiSdk _cheqiSdk;
    
    public ReceiptController(CheqiSdk cheqiSdk)
    {
        _cheqiSdk = cheqiSdk;
    }
    
    [HttpPost("receipts")]
    public async Task<IActionResult> CreateReceipt([FromBody] ReceiptRequest request)
    {
        var result = await _cheqiSdk.ReceiptService.ProcessCompleteReceiptAsync(
            request.Customer,
            request.Receipt
        );
        
        return Ok(result);
    }
}
```

### Batch Processing

Process multiple receipts efficiently:

```csharp
var receipts = GetPendingReceipts(); // Your data source

var tasks = receipts.Select(async receipt =>
{
    try
    {
        var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
            receipt.Customer,
            receipt.Data
        );
        
        return new { Receipt = receipt, Result = result, Success = true };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process receipt {Id}", receipt.Id);
        return new { Receipt = receipt, Result = (ReceiptResult)null, Success = false };
    }
});

var results = await Task.WhenAll(tasks);

var successful = results.Count(r => r.Success);
var failed = results.Count(r => !r.Success);

Console.WriteLine($"Processed {successful} receipts, {failed} failed");
```

### Custom Logging

Integrate with your logging framework:

```csharp
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .AddDebug()
        .SetMinimumLevel(LogLevel.Debug);
});

var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("your-api-key")
    .Build();

// SDK automatically uses ILogger<T> for all services
```

### Cancellation Token Support

All async methods support cancellation:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
        customer,
        receipt,
        cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out after 30 seconds");
}
```

### Manual Workflow Control

For advanced scenarios, control each step manually:

```csharp
// Step 1: Match customer
var matchResponse = await sdk.MatchingService.MatchCustomerAsync(customer);

if (!matchResponse.CustomerFound)
{
    // Fallback to email
    return await sdk.ReceiptService.SendReceiptViaEmailAsync(
        customer.Email,
        receipt
    );
}

// Step 2: Generate template
var template = await sdk.ReceiptService.GenerateReceiptTemplateAsync(receiptRequest);

// Step 3: Parse and canonicalize
var purchaseReceipt = JsonSerializer.Deserialize<PurchaseReceipt>(template);
var canonicalJson = sdk.EncryptionService.CanonicalizeJson(template);
var templateHash = HashUtils.Sha256Hex(canonicalJson);

// Step 4: Create encrypted receipts
var encryptedReceipts = sdk.ReceiptService.CreateEncryptedReceipts(
    canonicalJson,
    matchResponse.Recipients
);

// Step 5: Send to backend
var response = await sdk.ReceiptService.SendEncryptedReceiptsAsync(
    encryptedReceipts,
    templateHash,
    matchResponse
);

Console.WriteLine($"Delivered to {response.RecipientCount} devices");
```

## Testing

### Unit Testing

Mock the SDK services for unit tests:

```csharp
using Moq;
using Xunit;

public class ReceiptProcessorTests
{
    [Fact]
    public async Task ProcessReceipt_Success_ReturnsResult()
    {
        // Arrange
        var mockSdk = new Mock<CheqiSdk>();
        var mockReceiptService = new Mock<IReceiptService>();
        
        mockReceiptService
            .Setup(x => x.ProcessCompleteReceiptAsync(
                It.IsAny<IdentificationDetails>(),
                It.IsAny<ReceiptTemplateRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ReceiptResult.Success("receipt-123", 1));
        
        // Act
        var processor = new ReceiptProcessor(mockSdk.Object);
        var result = await processor.ProcessAsync(customer, receipt);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.ReceiptCount);
    }
}
```

### Integration Testing

Test against sandbox environment:

```csharp
[Fact]
public async Task IntegrationTest_ProcessReceipt()
{
    var sdk = CheqiSdk.Builder()
        .ApiEndpoint(Environment.Sandbox)
        .ApiKey(Environment.GetEnvironmentVariable("CHEQI_TEST_API_KEY"))
        .Build();
    
    var customer = new IdentificationDetails
    {
        Email = "test@example.com"
    };
    
    var receipt = CreateTestReceipt();
    
    var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
        customer,
        receipt
    );
    
    Assert.NotNull(result);
}
```

## Best Practices

### 1. Reuse SDK Instance

Create one SDK instance and reuse it throughout your application:

```csharp
// ✅ Good - Singleton pattern
public class ReceiptService
{
    private static readonly CheqiSdk _sdk = CheqiSdk.Builder()
        .ApiEndpoint(Environment.Production)
        .ApiKey("your-api-key")
        .Build();
}

// ❌ Bad - Creating new instances
public async Task ProcessReceipt()
{
    var sdk = CheqiSdk.Builder().Build(); // Don't do this repeatedly
}
```

### 2. Handle All Result Cases

Always check result status:

```csharp
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(customer, receipt);

if (result.IsSuccess)
{
    // Receipt delivered successfully
    await UpdateDatabase(result.ReceiptId, "delivered");
}
else if (!result.RecipientsFound)
{
    // No Cheqi users found - consider email fallback
    await SendEmailReceipt(customer.Email, receipt);
}
else if (result.HasError)
{
    // Processing error occurred
    _logger.LogError("Receipt processing failed: {Error}", result.ErrorMessage);
    await QueueForRetry(receipt);
}
```

### 3. Use Appropriate Identifiers

Choose the most reliable identifier available:

```csharp
// Priority order (most to least reliable):
// 1. PAR (Payment Account Reference) - most stable
// 2. IBAN - good for bank transfers
// 3. PAN (Primary Account Number) - can change with card replacement
// 4. Email - fallback option

var customer = new IdentificationDetails
{
    CardDetails = new CardDetails
    {
        Par = transaction.Par,           // Best option
        Pan = transaction.Pan,           // Fallback if PAR unavailable
        CardProvider = CardProvider.VISA
    },
    PaymentAccountDetails = new PaymentAccountDetails
    {
        Iban = transaction.Iban          // For bank transfers
    },
    Email = transaction.CustomerEmail    // Last resort
};
```

### 4. Implement Retry Logic

For transient failures, implement retry with exponential backoff:

```csharp
public async Task<ReceiptResult> ProcessWithRetry(
    IdentificationDetails customer,
    ReceiptTemplateRequest receipt,
    int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await sdk.ReceiptService.ProcessCompleteReceiptAsync(
                customer,
                receipt
            );
        }
        catch (CheqiApiException ex) when (ex.HttpStatusCode >= 500)
        {
            if (attempt == maxRetries) throw;
            
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            _logger.LogWarning("Attempt {Attempt} failed, retrying in {Delay}s", 
                attempt, delay.TotalSeconds);
            
            await Task.Delay(delay);
        }
    }
    
    throw new InvalidOperationException("Should not reach here");
}
```

### 5. Validate Data Before Sending

Ensure data quality before API calls:

```csharp
public async Task<ReceiptResult> ProcessReceipt(
    IdentificationDetails customer,
    ReceiptTemplateRequest receipt)
{
    // Validate customer has at least one identifier
    if (string.IsNullOrWhiteSpace(customer.Email) &&
        customer.CardDetails == null &&
        customer.PaymentAccountDetails == null)
    {
        throw new ArgumentException("Customer must have at least one identifier");
    }
    
    // Validate receipt data
    if (receipt.Products == null || !receipt.Products.Any())
    {
        throw new ArgumentException("Receipt must contain at least one product");
    }
    
    if (receipt.TotalAmount <= 0)
    {
        throw new ArgumentException("Total amount must be positive");
    }
    
    // Validate tax calculations
    var calculatedTax = receipt.Products.Sum(p => p.Tax?.TaxAmount ?? 0);
    if (Math.Abs(calculatedTax - receipt.TotalTaxAmount) > 0.01m)
    {
        _logger.LogWarning("Tax mismatch: calculated {Calculated}, provided {Provided}",
            calculatedTax, receipt.TotalTaxAmount);
    }
    
    return await sdk.ReceiptService.ProcessCompleteReceiptAsync(customer, receipt);
}
```

### 6. Secure API Key Storage

Never hardcode API keys:

```csharp
// ✅ Good - Use configuration
var apiKey = configuration["Cheqi:ApiKey"];
var apiKey = Environment.GetEnvironmentVariable("CHEQI_API_KEY");

// ❌ Bad - Hardcoded
var apiKey = "sk_live_abc123"; // Never do this!
```

### 7. Understand Hash Consistency

Receipt hashes must be identical across platforms for verification:

```csharp
// The SDK ensures hash consistency by:
// 1. Using compact JSON (no whitespace)
// 2. Preserving backend JSON structure
// 3. Using culture-invariant number formatting
// 4. Following RFC8785 canonicalization

// Same receipt data will produce identical hashes on:
// - .NET SDK
// - Java SDK  
// - iOS SDK

// This enables:
// - Cross-platform verification
// - Tamper detection
// - Third-party validation
```

### 8. Monitor and Log

Implement comprehensive logging:

```csharp
public async Task<ReceiptResult> ProcessReceipt(
    IdentificationDetails customer,
    ReceiptTemplateRequest receipt)
{
    var sw = Stopwatch.StartNew();
    
    _logger.LogInformation("Processing receipt {ReceiptId} for customer", 
        receipt.ReceiptId);
    
    try
    {
        var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
            customer,
            receipt
        );
        
        sw.Stop();
        
        _logger.LogInformation(
            "Receipt {ReceiptId} processed successfully in {Duration}ms. " +
            "Delivered to {Count} devices",
            receipt.ReceiptId,
            sw.ElapsedMilliseconds,
            result.ReceiptCount
        );
        
        return result;
    }
    catch (Exception ex)
    {
        sw.Stop();
        
        _logger.LogError(ex,
            "Failed to process receipt {ReceiptId} after {Duration}ms",
            receipt.ReceiptId,
            sw.ElapsedMilliseconds
        );
        
        throw;
    }
}
```

## Troubleshooting

### Common Issues

#### 1. Authentication Failures

```
Error: Invalid or expired access token
```

**Solution**: Ensure your API key is valid and not expired:

```csharp
// Check API key format
if (!apiKey.StartsWith("sk_live_") && !apiKey.StartsWith("sk_test_"))
{
    throw new InvalidOperationException("Invalid API key format");
}
```

#### 2. Customer Not Found

```
Result: RecipientsFound = false
```

**Solution**: Customer doesn't have Cheqi app. Use email fallback:

```csharp
if (!result.RecipientsFound && !string.IsNullOrEmpty(customer.Email))
{
    return await sdk.ReceiptService.SendReceiptViaEmailAsync(
        customer.Email,
        receipt
    );
}
```

#### 3. Timeout Errors

```
Error: The operation was canceled
```

**Solution**: Increase timeout or check network connectivity:

```csharp
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("your-api-key")
    .TimeoutSeconds(120) // Increase from default 30s
    .Build();
```

#### 4. Validation Errors

```
Error: Total amount must be positive
```

**Solution**: Validate data before sending:

```csharp
// Ensure amounts are positive
receipt.TotalAmount = Math.Abs(receipt.TotalAmount);

// Ensure product quantities are valid
foreach (var product in receipt.Products)
{
    if (product.Quantity <= 0)
        product.Quantity = 1;
}
```

#### 5. Encryption Errors

```
Error: Failed to encrypt receipt
```

**Solution**: Ensure recipient has valid public key:

```csharp
var matchResponse = await sdk.MatchingService.MatchCustomerAsync(customer);

if (matchResponse.Recipients.Any(r => string.IsNullOrEmpty(r.PublicKey)))
{
    _logger.LogWarning("Some recipients missing public keys");
}
```

#### 6. Hash Mismatch Errors

```
Error: Hash verification failed
```

**Solution**: This indicates JSON canonicalization differences. The SDK has been updated to prevent this:

- Ensure you're using the latest SDK version
- Backend must be configured with `default-property-inclusion: non_null`
- Decimal values use invariant culture formatting
- JSON is compact (no whitespace)

If issues persist, check that:

```csharp
// Verify SDK is using correct JSON options
var sdk = CheqiSdk.Builder()
    .ApiEndpoint(Environment.Production)
    .ApiKey("your-api-key")
    .Build();

// The SDK automatically handles canonicalization
// No manual configuration needed
```

### Debug Mode

Enable detailed logging for troubleshooting:

```csharp
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug) // Enable debug logs
        .AddFilter("Cheqi.Sdk", LogLevel.Trace); // Trace SDK internals
});
```

### Network Diagnostics

Test connectivity to Cheqi API:

```csharp
public async Task<bool> TestConnectivity()
{
    try
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://api.cheqi.io/health");
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Connectivity test failed");
        return false;
    }
}
```

## Performance Optimization

### Connection Pooling

The SDK automatically uses connection pooling via `HttpClient`. Reuse the SDK instance:

```csharp
// ✅ Singleton - connections are pooled
private static readonly CheqiSdk _sdk = CreateSdk();

// ❌ Multiple instances - connections not pooled
public async Task Process()
{
    var sdk = CreateSdk(); // Don't create new instances
}
```

### Parallel Processing

Process multiple receipts in parallel:

```csharp
var receipts = GetBatchReceipts();

var options = new ParallelOptions
{
    MaxDegreeOfParallelism = 10 // Limit concurrent requests
};

await Parallel.ForEachAsync(receipts, options, async (receipt, ct) =>
{
    try
    {
        await sdk.ReceiptService.ProcessCompleteReceiptAsync(
            receipt.Customer,
            receipt.Data,
            ct
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process receipt {Id}", receipt.Id);
    }
});
```

### Caching

Cache customer matching results:

```csharp
private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

public async Task<RecipientResolutionResponse> GetOrMatchCustomer(
    IdentificationDetails customer)
{
    var cacheKey = $"customer_{customer.GetHashCode()}";
    
    if (_cache.TryGetValue(cacheKey, out RecipientResolutionResponse cached))
    {
        return cached;
    }
    
    var result = await sdk.MatchingService.MatchCustomerAsync(customer);
    
    _cache.Set(cacheKey, result, TimeSpan.FromMinutes(15));
    
    return result;
}
```

## Support

- **Documentation**: [https://docs.cheqi.io](https://docs.cheqi.io)
- **API Reference**: [https://docs.cheqi.io/api-reference/](https://api.cheqi.io/docs)
- **Email**: support@cheqi.io
- **GitHub Issues**: [https://github.com/cheqi/cheqi-sdk-dotnet/issues](https://github.com/cheqi/cheqi-sdk-dotnet/issues)

## License

MIT License - see [LICENSE](LICENSE) for details.
