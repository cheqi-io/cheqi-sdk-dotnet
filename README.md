# Cheqi .NET SDK

[![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

The Cheqi .NET SDK provides end-to-end encrypted receipt and credit note processing capabilities for suppliers, enabling secure digital receipt delivery to customers and handling returns/refunds through the Cheqi platform with bidirectional encrypted communication.

## Features

- 🔐 **End-to-End Encryption**: Hybrid RSA+AES encryption for secure receipt delivery
- 🎯 **Customer Matching**: Match customers using payment identifiers (cards, IBANs, etc.)
- 📄 **Receipt Generation**: Create UBL-compliant receipt templates from transaction data
- 💳 **Credit Note Processing**: Complete returns, refunds, and cancellations with encryption
- 🔄 **Bidirectional Flow**: Send receipts to customers, receive encrypted return requests
- 🏢 **Company Provisioning**: Provision companies and receive immediate API access
- 🏪 **Store Management**: Full CRUD operations for store/location management
- 🌐 **Multi-Environment Support**: Sandbox, Test, and Production environments
- 🔄 **Automatic Retry Logic**: Built-in retry mechanisms for network operations
- 📊 **RFC8785 Canonicalization**: Consistent JSON canonicalization for integrity verification
- 🏗️ **Fluent API**: Builder pattern design for easy configuration
- ⚡ **Async/Await**: Modern asynchronous programming support

## Installation

### NuGet Package Manager

```bash
Install-Package Cheqi.SDK -Version 1.0.0
```

### .NET CLI

```bash
dotnet add package Cheqi.SDK --version 1.0.0
```

### PackageReference

Add to your `.csproj` file:

```xml
<PackageReference Include="Cheqi.SDK" Version="1.0.0" />
```

## Quick Start

```csharp
using Cheqi.SDK;
using Cheqi.SDK.Config;
using Cheqi.SDK.Models;
using Cheqi.SDK.Enums;

// 1. Initialize the SDK
var sdk = new CheqiSdkBuilder()
    .WithEnvironment(Environment.Sandbox)
    .Build();

// 2. Identify the customer (using card payment details)
var customer = new IdentificationDetails
{
    PaymentType = PaymentType.CardPayment,
    CardDetails = new CardDetails
    {
        PaymentAccountReference = "PAR123456789",  // From payment terminal
        CardProvider = CardProvider.Visa
    },
    CustomerEmail = "customer@example.com"  // Fallback if you don't have a PAR
};

// 3. Build the receipt
var receipt = new ReceiptTemplateRequest
{
    DocumentNumber = "INV-001",
    Currency = "EUR",
    TotalAmount = 12.10m,
    TotalTaxAmount = 2.10m,
    Products = new List<Product>
    {
        new Product
        {
            Name = "Coffee",
            Quantity = 1.0,
            UnitCode = UnitCode.One,
            UnitPrice = 10.00m,
            Total = 12.10m,
            Taxes = new List<ProductTax>
            {
                new ProductTax { Rate = 21.0, Type = "VAT", Amount = 2.10m }
            }
        }
    }
};

// 4. Send the receipt (one method does everything)
var result = await sdk.ReceiptService
    .ProcessCompleteReceiptAsync(customer, receipt, accessToken);

if (result.IsSuccess)
{
    Console.WriteLine("Receipt sent!");
}
```

**Prerequisites:**
- **`companyId`** - Your company UUID (from provisioning or Cheqi dashboard)
- **`accessToken`** - OAuth token with receipt scopes (see [Authentication](#authentication))

## Environment Configuration

The SDK supports multiple environments:

| Environment              | URL                            | Description                 |
|--------------------------|--------------------------------|-----------------------------|
| `Environment.Sandbox`    | `https://sandbox.api.cheqi.io` | Development                 |
| `Environment.Test`       | `https://test.api.cheqi.io`    | Test environment            |
| `Environment.Production` | `https://api.cheqi.io`         | Live production environment |

### SDK Initialization

```csharp
using Cheqi.SDK;
using Cheqi.SDK.Config;

var sdk = new CheqiSdkBuilder()
    .WithEnvironment(Environment.Sandbox)  // Use Sandbox for testing
    .Build();
```

**Custom endpoint** (for local development):
```csharp
var sdk = new CheqiSdkBuilder()
    .WithCustomEndpoint("http://localhost:8080")
    .Build();
```

### Processing Receipts

**One method handles everything** - customer matching, encryption, and delivery:

```csharp
using Cheqi.SDK.Models;
using Cheqi.SDK.Enums;
using System;
using System.Collections.Generic;

// Create identification details for customer matching
var identificationDetails = new IdentificationDetails
{
    PaymentType = PaymentType.CardPayment,
    CardDetails = new CardDetails
    {
        PaymentAccountReference = "PAR123456",
        CardProvider = CardProvider.Visa
    }
};

// Create receipt template request
var receiptRequest = new ReceiptTemplateRequest
{
    DocumentNumber = "INV-001",  // Invoice/receipt number
    IssueDate = DateTime.UtcNow,
    Currency = "EUR",  // ISO 4217 currency code
    InvoiceSubtotal = 100.00m,
    TotalBeforeTax = 100.00m,
    TotalAmount = 121.00m,
    TotalTaxAmount = 21.00m,  // Total tax (POS calculated)
    Products = new List<Product>
    {
        new Product
        {
            Name = "Laptop",
            Quantity = 1.0,
            UnitCode = UnitCode.One,  // Type-safe enum
            UnitPrice = 100.00m,
            Subtotal = 100.00m,
            Total = 121.00m,
            Taxes = new List<ProductTax>
            {
                new ProductTax { Rate = 21.0, Type = "VAT", Amount = 21.00m }
            }
        }
    },
    Taxes = new List<Tax>  // Receipt-level tax breakdown
    {
        new Tax
        {
            Rate = 21.0,
            Type = "VAT",
            TaxableAmount = 100.00m,  // Amount before tax
            Amount = 21.00m,          // Tax amount (100 × 0.21)
            Label = "VAT 21%"
        }
    }
};

// Process complete receipt (match + generate + encrypt + send)
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
    identificationDetails,  // Customer identification details
    receiptRequest,         // Receipt template data
    merchantId,             // Guid of the merchant
    accessToken             // OAuth access token
);

if (result.IsSuccess)
{
    Console.WriteLine("Receipt delivered successfully!");
    Console.WriteLine($"Receipts created: {result.ReceiptCount}");
}
else if (result.IsCustomerNotFound)
{
    // Customer not found - prompt for email and send request with email included
    Console.WriteLine("Customer not found. Prompt user for email to send receipt.");
}
else
{
    Console.WriteLine($"Processing failed: {result.Message}");
}
```

**This method automatically:**
1. **Matches the customer** using the provided identification details (card PAR, IBAN, email, etc.)
2. **Generates a receipt template** from the request data (only if customer is found)
3. **Creates encrypted receipts** for all customer devices
4. **Sends the receipts** to the Cheqi backend for delivery

### Email Fallback

**You can include an email address in your initial request** for automatic fallback if the customer is not found:

```csharp
// Add email to identification details for fallback
var identificationDetails = new IdentificationDetails
{
    PaymentType = PaymentType.CardPayment,
    CardDetails = new CardDetails
    {
        PaymentAccountReference = "PAR123456",
        CardProvider = CardProvider.Visa
    },
    CustomerEmail = "customer@example.com"  // Email fallback
};

// Process receipt - email will be used if customer not found
var result = await sdk.ReceiptService.ProcessCompleteReceiptAsync(
    identificationDetails,
    receiptRequest,
    merchantId,
    accessToken
);

if (result.IsSuccess)
{
    Console.WriteLine("Receipt delivered successfully!");
}
else if (result.IsCustomerNotFound)
{
    Console.WriteLine("Customer not found - please collect email for receipt delivery");
}
else
{
    Console.WriteLine($"Failed: {result.Message}");
}
```

**Email Fallback Behavior:**
- ✅ Customer **found** → Encrypted digital receipt delivered to their Cheqi app (email is ignored)
- ✅ Customer **not found** + **email provided** → PDF receipt sent via email (handled by Cheqi)
- ❌ Customer **not found** + **no email** → Returns `IsCustomerNotFound = true`

**Tip:** If you collect email at checkout, include it in the initial request to avoid a second API call.

## Advanced Usage

### Custom Configuration

```csharp
var sdk = new CheqiSdkBuilder()
    .WithEnvironment(Environment.Production)
    .WithSupplierCredentials("client-id", "client-secret")
    .WithTimeout(TimeSpan.FromSeconds(60))  // Custom timeout
    .WithMaxRetries(5)                      // Custom retry count
    .Build();
```

### Building Products

The SDK provides a fluent API for building product line items with type-safe unit codes:

```csharp
using Cheqi.SDK.Models;
using Cheqi.SDK.Enums;
using System;

// Simple product
var laptop = new Product
{
    Name = "MacBook Pro 14\"",
    Sku = "MBP-14-2024",
    Quantity = 1.0,
    BaseQuantity = 1.0,        // Mandatory: 1.0 for simple items
    UnitCode = UnitCode.One,   // Type-safe enum
    UnitPrice = 2499.00m,
    Subtotal = 2499.00m,
    Total = 3023.79m,
    Taxes = new List<ProductTax>
    {
        new ProductTax { Rate = 21.0, Type = "VAT", Amount = 524.79m }
    }
};

// Pre-packaged product (10 packages × 500g cheese)
var cheese = new Product
{
    Name = "Gouda Cheese",
    Sku = "CHEESE-500G",
    Quantity = 10.0,           // 10 packages
    BaseQuantity = 500.0,      // 500g per package (total: 5000g)
    UnitCode = UnitCode.Gram,
    UnitPrice = 4.99m,         // Price per package
    Subtotal = 49.90m,
    Total = 60.38m,
    Taxes = new List<ProductTax>
    {
        new ProductTax { Rate = 21.0, Type = "VAT", Amount = 10.48m }
    }
};

// Weight-based product
var apples = new Product
{
    Name = "Organic Apples",
    Sku = "APPLE-ORG",
    Quantity = 2.5,
    BaseQuantity = 1.0,        // Always 1.0 for weight-based
    UnitCode = UnitCode.Kilogram,
    UnitPrice = 3.99m,
    Discounts = new List<Discount>
    {
        new Discount { Amount = 1.00m, Reason = "Loyalty discount" }
    },
    Charges = new List<Charge>
    {
        new Charge { Amount = 0.50m, Reason = "Packaging fee" }
    },
    Subtotal = 9.48m,
    Total = 11.47m,
    Taxes = new List<ProductTax>
    {
        new ProductTax { Rate = 21.0, Type = "VAT", Amount = 1.99m }
    }
};

// Time-based product (subscription with period)
var subscription = new Product
{
    Name = "Premium Subscription",
    Sku = "SUB-PREMIUM-M",
    Quantity = 1.0,
    BaseQuantity = 1.0,
    UnitCode = UnitCode.Month,
    UnitPrice = 29.99m,
    Subtotal = 29.99m,
    Total = 36.29m,
    Taxes = new List<ProductTax>
    {
        new ProductTax { Rate = 21.0, Type = "VAT", Amount = 6.30m }
    },
    Period = new Period  // Optional: for subscriptions, rentals
    {
        StartDate = new DateTime(2024, 1, 1),
        EndDate = new DateTime(2024, 1, 31),
        Description = "January 2024"
    }
};

// Hourly rental with precise times
var carRental = new Product
{
    Name = "Tesla Model 3 Rental",
    Quantity = 3.0,
    BaseQuantity = 1.0,
    UnitCode = UnitCode.Hour,
    UnitPrice = 25.00m,
    Subtotal = 75.00m,
    Total = 90.75m,
    Taxes = new List<ProductTax>
    {
        new ProductTax { Rate = 21.0, Type = "VAT", Amount = 15.75m }
    },
    Period = new Period
    {
        StartDate = new DateTime(2024, 12, 1, 14, 0, 0),  // 2:00 PM
        EndDate = new DateTime(2024, 12, 1, 17, 0, 0),    // 5:00 PM
        Description = "3-hour rental"
    }
};
```

**Base Quantity Rules:**
- **Simple items**: Set `BaseQuantity` to `1.0` (e.g., 1 laptop, 2.5kg apples)
- **Pre-packaged items**: Set `BaseQuantity` to package size (e.g., 500 for 500g packages)
- **Total quantity**: `Quantity × BaseQuantity` (e.g., 10 × 500g = 5000g)

**Period Support:**
- Use `Period` for time-based products: subscriptions, rentals, utilities, service contracts
- Leave empty for one-time purchases
- Includes `StartDate`, `EndDate`, and optional `Description`

**Available Unit Codes:**
- **Common**: `One`, `Each`, `Set`, `Pair`, `Dozen`
- **Weight**: `Kilogram`, `Gram`, `Pound`, `Ounce`
- **Volume**: `Liter`, `Milliliter`, `GallonUs`, `GallonUk`
- **Length**: `Meter`, `Centimeter`, `Inch`, `Foot`
- **Time**: `Second`, `Minute`, `Hour`, `Day`, `Week`, `Month`, `Year`
- **Packaging**: `Box`, `Carton`, `Bottle`, `Can`, `Bag`, `Pallet`
- **Service**: `ServiceUnit`, `Activity`, `Job`

See `UnitCode` enum for the complete list of 70+ Peppol-compliant codes.

### Tax Breakdown

The `Tax` model supports detailed tax reporting with taxable amounts:

```csharp
using Cheqi.SDK.Models;

// Single tax rate
var vat = new Tax
{
    Rate = 21.0,                    // 21% VAT
    Type = "VAT",
    TaxableAmount = 100.00m,        // €100 taxable amount
    Amount = 21.00m,                // €21 tax (100 × 0.21)
    Label = "VAT 21%"
};

// Multiple tax rates on a receipt
var receipt = new ReceiptTemplateRequest
{
    DocumentNumber = "INV-001",
    Currency = "EUR",
    TotalBeforeTax = 10000.00m,
    TotalTaxAmount = 1865.00m,
    TotalAmount = 11865.00m,
    
    // Tax breakdown by rate
    Taxes = new List<Tax>
    {
        new Tax
        {
            Rate = 19.0,
            Type = "VAT",
            TaxableAmount = 8500.00m,  // €8500 @ 19%
            Amount = 1615.00m,         // = €1615
            Label = "VAT 19%"
        },
        new Tax
        {
            Rate = 9.0,
            Type = "VAT",
            TaxableAmount = 1500.00m,  // €1500 @ 9% (reduced)
            Amount = 135.00m,          // = €135
            Label = "VAT 9% (reduced)"
        }
    }
};
```

**Tax Fields:**
- **`Rate`** (required): Tax percentage (e.g., 21.0 for 21%)
- **`Type`** (required): Tax type (e.g., "VAT", "GST", "Sales Tax")
- **`TaxableAmount`** (optional): Base amount before tax - useful for tax reporting
- **`Amount`** (optional): Calculated tax amount
- **`Label`** (optional): Display label (e.g., "VAT 21%")

### Direct Service Access

```csharp
// Access individual services for advanced operations
var encryption = sdk.EncryptionService;
var decryption = sdk.DecryptionService;
var matching = sdk.MatchingService;
var receipts = sdk.ReceiptService;
var creditNotes = sdk.CreditNoteService;
var companies = sdk.CompanyService;
var stores = sdk.StoreService;
var apiClient = sdk.ApiClient;
```

### Company Provisioning

The `CompanyService` enables trusted POS systems to provision companies and receive immediate API access without waiting for merchant OAuth approval. Requires a ClientApplication with `partnerTier`.

```csharp
using Cheqi.SDK.Models.Company;
using System;

// Build company details
var company = new Company
{
    CompanyName = "My Store BV",
    CompanyLegalName = "My Store B.V.",
    ChamberOfCommerceNumber = "12345678",
    CompanyEmail = "info@mystore.nl",
    Address = new Address
    {
        StreetName = "Hoofdstraat 123",
        CityName = "Amsterdam",
        PostalZone = "1012AB",
        CountryIsoCode = "NL"
    }
};

// Provision the company
var response = await sdk.CompanyService
    .ProvisionCompanyAsync(company, "owner@mystore.nl", clientApplicationToken);

if (response.IsProvisioned)
{
    // New company created - use tokens immediately
    var accessToken = response.AccessToken;
    var companyId = response.CompanyId;
    Console.WriteLine($"Company provisioned: {companyId}");
}
else if (response.IsAlreadyExists)
{
    // Company exists - initiate OAuth flow for authorization
    var companyId = response.CompanyId;
    Console.WriteLine($"Company exists, OAuth required: {companyId}");
}
```

**Provisioning Response States:**
- ✅ **`IsProvisioned`** - New company created, tokens returned for immediate use
- ⚠️ **`IsAlreadyExists`** - Company exists, OAuth flow required for authorization

### Store Management

The `StoreService` provides full CRUD operations for managing stores/locations. Requires `WRITE_STORES` scope.

```csharp
using Cheqi.SDK.Models.Company;
using System;
using System.Collections.Generic;

// Create a new store
var storeRequest = new CreateStoreRequest
{
    StoreName = "My Store Amsterdam",
    StoreCode = "STORE-001",
    Address = new Address
    {
        StreetName = "Kalverstraat 1",
        CityName = "Amsterdam",
        PostalZone = "1012NX",
        CountryIsoCode = "NL"
    },
    PhoneNumber = "+31201234567",
    Email = "amsterdam@mystore.nl",
    OpeningHours = "Mon-Sat: 09:00-18:00"
};

var store = await sdk.StoreService
    .CreateStoreAsync(companyId, storeRequest, accessToken);
Console.WriteLine($"Store created: {store.Id}");

// List all stores for a company
var stores = await sdk.StoreService
    .GetStoresAsync(companyId, accessToken);

// Get only active stores
var activeStores = await sdk.StoreService
    .GetActiveStoresAsync(companyId, accessToken);

// Get a specific store
var store = await sdk.StoreService
    .GetStoreAsync(companyId, storeId, accessToken);

// Update a store
var updateRequest = new CreateStoreRequest
{
    StoreName = "My Store Amsterdam - Central",
    StoreCode = "STORE-001",
    Address = new Address
    {
        StreetName = "Kalverstraat 1",
        CityName = "Amsterdam",
        PostalZone = "1012NX",
        CountryIsoCode = "NL"
    }
};

var updatedStore = await sdk.StoreService
    .UpdateStoreAsync(companyId, storeId, updateRequest, accessToken);

// Activate/Deactivate a store
await sdk.StoreService.ActivateStoreAsync(companyId, storeId, accessToken);
await sdk.StoreService.DeactivateStoreAsync(companyId, storeId, accessToken);

// Delete a store
await sdk.StoreService.DeleteStoreAsync(companyId, storeId, accessToken);
```

**Store Operations:**
- **`CreateStoreAsync()`** - Create a new store for a company
- **`GetStoresAsync()`** - List all stores for a company
- **`GetActiveStoresAsync()`** - List only active stores
- **`GetStoreAsync()`** - Get a specific store by ID
- **`UpdateStoreAsync()`** - Update store details
- **`ActivateStoreAsync()`** / **`DeactivateStoreAsync()`** - Toggle store status
- **`DeleteStoreAsync()`** - Remove a store

### Credit Note Processing

The SDK supports complete credit note processing for returns, refunds, and cancellations with the same end-to-end encryption as receipts.

#### Processing Credit Notes

**One method handles everything** - customer identification via receipt ID, encryption, and delivery:

```csharp
using Cheqi.SDK.Models.CreditNote;
using Cheqi.SDK.Enums;
using System;
using System.Collections.Generic;

// 1. Identify customer via original receipt ID
var identificationDetails = new IdentificationDetails
{
    CheqiReceiptId = Guid.Parse("original-receipt-uuid")  // UUID from original receipt
};

// 2. Build the credit note template
var creditNote = new CreditNoteTemplateRequest
{
    DocumentNumber = "CN-2024-001",
    IssueDate = DateTime.UtcNow,
    Currency = "EUR",
    OriginatorDocumentReference = "original-receipt-number",  // Original invoice number
    TotalAmount = 121.00m,
    TotalTaxAmount = 21.00m,
    
    // Return items
    ReturnLineItems = new List<ReturnLineItem>
    {
        new ReturnLineItem
        {
            Name = "Defective Laptop",
            Quantity = 1.0,
            UnitPrice = 100.00m,
            Total = 121.00m,
            TaxRate = 21.0,
            TaxAmount = 21.00m,
            ReturnCondition = ReturnCondition.Defective,
            ReturnReason = "Screen not working"
        }
    },
    
    // Refund preference
    RefundPreference = RefundPreference.BankAccount,
    RefundBankAccount = new RefundBankAccount
    {
        Iban = "NL91ABNA0417164300",
        AccountHolderName = "John Doe"
    },
    
    Note = "Refund for defective product"
};

// 3. Process credit note (match customer + generate + encrypt + send)
var result = await sdk.CreditNoteService
    .ProcessCompleteCreditNoteAsync(identificationDetails, creditNote, accessToken);

if (result.IsSuccess)
{
    Console.WriteLine("Credit note delivered successfully!");
    Console.WriteLine($"Credit notes created: {result.ReceiptCount}");
}
else if (result.IsCustomerNotFound)
{
    Console.WriteLine("Customer not found");
}
else
{
    Console.WriteLine($"Processing failed: {result.Message}");
}
```

**This method automatically:**
1. **Identifies the customer** using the `CheqiReceiptId` from the original receipt
2. **Generates a credit note template** from the request data
3. **Creates encrypted credit notes** for all customer devices
4. **Sends the credit notes** to the Cheqi backend for delivery

#### Receiving Credit Note Requests (Merchant Side)

When a customer initiates a return through the Cheqi app, merchants receive an encrypted credit note request via webhook:

```csharp
using Cheqi.SDK.Models.CreditNote;

// Received from webhook
var encryptedRequest = // ... from webhook

// Decrypt and parse the credit note request
var request = await sdk.CreditNoteService
    .DecryptCreditNoteRequestAsync(encryptedRequest);

// Access the request details
Console.WriteLine($"Receipt ID: {request.CheqiReceiptId}");
Console.WriteLine($"Return reason: {request.ReturnReason}");
Console.WriteLine($"Refund preference: {request.RefundPreference}");

// Process the return based on your business logic
if (request.RefundPreference == RefundPreference.BankAccount)
{
    var bankAccount = request.RefundBankAccount;
    Console.WriteLine($"Refund to IBAN: {bankAccount.Iban}");
    // Process bank refund...
}
else
{
    // Process refund to original payment method...
}
```

**Credit Note Request Flow:**
1. Customer initiates return in Cheqi app
2. Cheqi encrypts request with merchant's public key
3. Webhook delivers encrypted request to merchant
4. Merchant decrypts using SDK
5. Merchant processes return/refund
6. Merchant sends credit note back to customer (using `ProcessCompleteCreditNoteAsync`)

## Error Handling

The SDK provides comprehensive error handling with custom exceptions:

```csharp
using Cheqi.SDK.Exceptions;
using System;

try
{
    var response = await sdk.MatchingService
        .MatchCustomerAsync(request, accessToken);
}
catch (CheqiApiException ex)
{
    Console.Error.WriteLine($"API Error: {ex.Message}");
    Console.Error.WriteLine($"Status Code: {ex.StatusCode}");
}
catch (CheqiSdkException ex)
{
    Console.Error.WriteLine($"SDK Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Security Features

### Encryption

- **Hybrid Encryption**: RSA-OAEP for key exchange + AES-256-GCM for data
- **Forward Secrecy**: Unique AES keys for each receipt and credit note
- **Integrity Protection**: Built-in authentication tags
- **Multi-Device Support**: Encrypt once, deliver to multiple devices
- **Bidirectional Security**: Both receipts (merchant→customer) and credit note requests (customer→merchant) are encrypted end-to-end

### Key Management

- **RSA Key Pairs**: 2048-bit minimum key size
- **Secure Key Storage**: Integration with platform key stores
- **Key Rotation**: Support for key lifecycle management

## Requirements

- **.NET**: 6.0 or higher
- **C#**: 10.0 or higher
- **Dependencies**: All dependencies are managed automatically via NuGet

## Dependencies

The SDK includes the following key dependencies:

- **System.Text.Json**: JSON processing and serialization
- **System.Net.Http**: HTTP client for API communication
- **System.Security.Cryptography**: Cryptographic operations

## Building from Source

```bash
# Clone the repository
git clone https://github.com/cheqi/cheqi-sdk-dotnet.git
cd cheqi-sdk-dotnet

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack
```

## Complete Example

Here's a complete example showing the entire receipt processing flow:

```csharp
using Cheqi.SDK;
using Cheqi.SDK.Config;
using Cheqi.SDK.Models;
using Cheqi.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReceiptExample
{
    public static async Task Main(string[] args)
    {
        // 1. Initialize SDK
        var sdk = new CheqiSdkBuilder()
            .WithEnvironment(Environment.Production)
            .WithTimeout(TimeSpan.FromSeconds(30))
            .WithMaxRetries(3)
            .Build();
        
        // 2. Create customer identification
        var customer = new IdentificationDetails
        {
            PaymentType = PaymentType.CardPayment,
            CardDetails = new CardDetails
            {
                PaymentAccountReference = "PAR_1234567890",
                CardProvider = CardProvider.Visa
            },
            CustomerEmail = "customer@example.com"  // Optional fallback
        };
        
        // 3. Build products
        var laptop = new Product
        {
            Name = "MacBook Pro 14\"",
            Sku = "MBP-14-2024",
            Quantity = 1.0,
            BaseQuantity = 1.0,
            UnitCode = UnitCode.One,
            UnitPrice = 2499.00m,
            Discounts = new List<Discount>
            {
                new Discount { Amount = 100.00m, Reason = "Black Friday" }
            },
            Subtotal = 2399.00m,
            Total = 2902.79m,
            Taxes = new List<ProductTax>
            {
                new ProductTax { Rate = 21.0, Type = "VAT", Amount = 503.79m }
            }
        };
        
        var apples = new Product
        {
            Name = "Organic Apples",
            Sku = "APPLE-ORG",
            Quantity = 2.5,
            BaseQuantity = 1.0,
            UnitCode = UnitCode.Kilogram,
            UnitPrice = 3.99m,
            Subtotal = 9.98m,
            Total = 12.08m,
            Taxes = new List<ProductTax>
            {
                new ProductTax { Rate = 21.0, Type = "VAT", Amount = 2.10m }
            }
        };
        
        // 4. Create receipt
        var receipt = new ReceiptTemplateRequest
        {
            DocumentNumber = "INV-2024-001",
            IssueDate = DateTime.UtcNow,
            Currency = "EUR",
            InvoiceSubtotal = 2408.98m,
            TotalBeforeTax = 2408.98m,
            TotalAmount = 2914.87m,
            TotalTaxAmount = 505.89m,
            Products = new List<Product> { laptop, apples },
            Taxes = new List<Tax>
            {
                new Tax
                {
                    Rate = 21.0,
                    Type = "VAT",
                    TaxableAmount = 2408.98m,  // Total taxable amount
                    Amount = 505.89m,          // Total tax (2408.98 × 0.21)
                    Label = "VAT 21%"
                }
            },
            Note = "Thank you for your purchase!"
        };
        
        // 5. Process receipt (one method does everything)
        try
        {
            var result = await sdk.ReceiptService
                .ProcessCompleteReceiptAsync(
                    customer,
                    receipt,
                    Guid.Parse("merchant-uuid"),
                    "access-token"
                );
            
            if (result.IsSuccess)
            {
                Console.WriteLine("✅ Receipt delivered successfully!");
                Console.WriteLine($"Receipts created: {result.ReceiptCount}");
            }
            else if (result.IsCustomerNotFound)
            {
                Console.WriteLine("⚠️ Customer not found - email receipt sent");
            }
            else
            {
                Console.Error.WriteLine($"❌ Failed: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

## Support

- **Documentation**: [https://docs.cheqi.io](https://docs.cheqi.io)
- **Support Email**: info@cheqi.io

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

**Cheqi** - Secure Digital Receipt Platform
