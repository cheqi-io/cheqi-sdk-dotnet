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

## Support

- **Documentation**: [https://docs.cheqi.io](https://docs.cheqi.io)
- **Email**: info@cheqi.io

## License

MIT License - see [LICENSE](LICENSE) for details.
