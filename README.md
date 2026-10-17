# Cheqi .NET SDK

The .NET SDK provides the same receipt issuance, encrypted downloads, match recovery, credit-note issuance, decryption, verification, and store management as the Java SDK 3.0.0. Its APIs use `Task`, cancellation tokens, `Guid` identifiers, `decimal` amounts, `System.Text.Json`, dependency injection, and `Microsoft.Extensions.Logging`.

Version 2.0 is a breaking migration from the old server-generated template flow. The library targets .NET 8 and is tested on .NET 8 and .NET 10.

## Installation and initialization

```sh
dotnet add package cheqi-sdk --version 3.0.0
```

```csharp
using Cheqi.Sdk;
using Cheqi.Sdk.Config;

using var sdk = CheqiSdk.Create(new CheqiSdkOptions
{
    Environment = CheqiApiEnvironment.Sandbox,
    ApiKey = "your-api-key",
    TimeoutSeconds = 30,
    MaxRetries = 3
});
```

Reuse the SDK instance. An injected `HttpClient` and `ILoggerFactory` remain owned by the caller. A client created internally is disposed with the SDK. An injected client can have its own handlers and retry policy; the SDK sets a request option to disable retries for encrypted digital submissions, which `RetryHandler` honors.

## Request options and OAuth

API-key authentication is the default. Every HTTP operation takes an optional `RequestOptions` before its `CancellationToken`. Use it to act on behalf of a connected company with its OAuth access token, or to override the timeout or retry count for one call:

```csharp
using Cheqi.Sdk.Http;

var result = await sdk.ReceiptService.IssueReceiptAsync(
    identification, payload, storeId,
    options: RequestOptions.WithAccessToken(accessToken),
    cancellationToken: cancellationToken);
```

The same `RequestOptions` can also set `Timeout` and `MaxRetries` for a single call, for example `new RequestOptions { AccessToken = accessToken, Timeout = TimeSpan.FromSeconds(60) }`.

Company access tokens are short-lived and bound to one Grant, so an integration that serves several companies holds one token per Grant. Request one with the client-credentials grant:

```csharp
var token = await sdk.OAuthService.RequestAccessTokenAsync(new ClientCredentialsTokenRequest
{
    ClientId = clientId,
    ClientSecret = clientSecret,
    GrantId = grantId,
    Scope = "receipt:issue"
}, cancellationToken: cancellationToken);

// token.AccessToken, token.ExpiresIn
```

The SDK does not cache or refresh tokens. Store each token with its expiry in your own token store, request a new one before it expires, and pass the current token with `RequestOptions`. A call with neither an access token nor a configured API key fails with `AUTHENTICATION_FAILED` before any request is sent.

`RequestOptions.Timeout` applies per call. An injected `HttpClient` keeps its own `Timeout` as an upper bound.

Register the SDK with dependency injection:

```csharp
using Cheqi.Sdk.Extensions;

services.AddCheqiSdk(options =>
{
    options.Environment = CheqiApiEnvironment.Production;
    options.ApiKey = configuration["Cheqi:ApiKey"];
});
```

Use `CustomApiEndpoint` for a local or private API. Set `ReceiptDownloadBaseUrl` for its customer-facing download frontend. Standard API environments configure these frontend origins automatically:

| Environment | API | Receipt downloads |
| --- | --- | --- |
| Sandbox | `https://sandbox.api.cheqi.io` | `https://sandbox.receipt.cheqi.io` |
| Test | `https://test.api.cheqi.io` | `https://test.receipt.cheqi.io` |
| Production | `https://api.cheqi.io` | `https://receipt.cheqi.io` |

## Issue a receipt

Use models from `Cheqi.Sdk.Models.Generated`. They represent the published OpenAPI contract, including payment context, fiscalization and jurisdictional metadata, taxes, discounts, charges, and product identifiers.

```csharp
using Cheqi.Sdk.Models.Generated;

var identification = new IdentificationDetails
{
    PaymentType = PaymentType.CARDPAYMENT,
    CardDetails = new CardDetails("payment-account-reference")
};

var payload = new ReceiptPayload(
    documentNumber: "POS-001",
    issueDate: DateTimeOffset.UtcNow,
    currency: "EUR",
    receiptSubtotal: 10m,
    totalBeforeTax: 10m,
    totalTaxAmount: 2.10m,
    totalAmount: 12.10m,
    taxesApplied: true,
    products: new List<Product>
    {
        new("Coffee", "coffee-1", 1, UnitCode.C62, 10m, 10m, 12.10m)
    });

var result = await sdk.ReceiptService.IssueReceiptAsync(
    identification, payload, cancellationToken: cancellationToken);
```

The SDK resolves a route, serializes the definitive values you supply, independently encrypts the same payload for each owner device with AES-256-GCM and RSA-OAEP-SHA256, and submits only ciphertext. It performs no receipt calculations or template enrichment. An accepted job can still be awaiting device processing; `result.Status` reports that backend status.

Pairing codes, card PARs, payment accounts, email addresses, and original Cheqi receipt IDs can identify customers. A payment type alone is valid anonymous payment context and can resolve a download fallback.

## Outcomes and recovery

| `result.State` | Meaning | Integration action |
| --- | --- | --- |
| `DigitalSubmitted` | Cheqi accepted the processing job | Save `CheqiReceiptId` and the submission |
| `DigitalPending` | Acceptance is uncertain or the original match is processing | Persist the prepared download, upload it, then resume the original match |
| `DigitalExpired` | An unsubmitted match expired | Follow the integration's expired-checkout policy |
| `DownloadFallback` | Download route accepted, prepared for later upload, or awaiting an envelope | Check `IsAccepted`, `PreparedDownload`, and `DownloadEnvelopeRequired` |
| `EmailFallback` | Explicit email route | Generate the final receipt locally and send it through `SendReceiptViaEmailAsync` |

`IsAccepted` and `IsSuccess` become true only after Cheqi acknowledges a submission or download upload. `Retryable` and `RetryAfterSeconds` guide recovery scheduling.

Network failures during matching prepare a download locally. Network failures, transient errors, in-progress responses, and malformed success responses during digital submission return `DigitalPending`. Permanent API errors propagate as `CheqiApiException`. Cancellation propagates as `OperationCanceledException`.

Persist the original fallback before displaying its URL or QR code. Storage, scheduling, retries, and retention belong to your integration:

```csharp
using System.Text.Json;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Serialization;

var jsonOptions = CheqiJson.CreateOptions();
if (result.PreparedDownload is { } prepared)
{
    // Persist this JSON with appropriate protection: its URL includes the secret key.
    var persistedJson = JsonSerializer.Serialize(prepared, jsonOptions);

    // In a later worker:
    var restored = JsonSerializer.Deserialize<PreparedReceiptDownload>(persistedJson, jsonOptions)!;
    await sdk.ReceiptService.UploadPreparedDownloadAsync(restored, cancellationToken: cancellationToken);
}

if (result.State == ReceiptIssueState.DigitalPending)
{
    var resumed = await sdk.ReceiptService.ResumeReceiptAsync(
        result.MatchId!, payload, cancellationToken: cancellationToken);
}
```

`ResumeReceiptAsync` makes one recovery attempt using only the original opaque `matchId` and definitive payload. It retrieves the original device snapshot, returns the original submission if already accepted, or submits for a still-matched digital route. It never rematches, retains matching identifiers, or generates replacement fallback credentials. Keep the original prepared download and URL across recovery attempts. Persist `storeId` as well when it was supplied for issuance.

Encrypted digital submissions bypass automatic retries. Matching, downloads, and store operations retain the configured retry policy. A raw in-progress submission raises `SubmissionInProgressException` with its retry delay.

For manual control, use `GetMatchAsync`, `GenerateEncryptedReceiptEnvelope` (accepts a resolution or a retrieved `MATCHED` status), and `SubmitEncryptedReceiptAsync`.

## Encrypted receipt downloads

For an explicit customer-without-Cheqi flow, skip matching:

```csharp
var download = await sdk.ReceiptService.IssueDownloadReceiptAsync(
    new IdentificationDetails { PaymentType = PaymentType.CASH },
    payload,
    cancellationToken: cancellationToken);
```

The SDK builds a `ReceiptEnvelope` containing the supplied payload and payment identification context, creates a random download ID and AES-256 key, and uploads ciphertext. The secret key stays in the URL fragment. Prepared recovery downloads contain the definitive payload without the matching identifiers.

When a resolved download route has no local payment type, `DownloadEnvelopeRequired` is true. Build the final `ReceiptEnvelope` locally and complete it:

```csharp
var completed = await sdk.ReceiptService.CompleteDownloadFallbackAsync(
    result, finalReceiptEnvelope, cancellationToken: cancellationToken);
```

`DownloadService` also exposes `GenerateDownloadLink`, `BuildDownloadUrl`, `ParseDownloadUrl`, `EncryptDownloadEnvelope`, and `DecryptDownloadEnvelope`. It uses unpadded base64url credentials and `Base64(iv || ciphertext || tag)` encryption framing shared with Java and JavaScript. Download credentials and prepared requests redact secrets in `ToString()`.

## Credit notes and customer returns

Issue definitive credit-note values through the separate encrypted endpoint:

```csharp
var creditNote = await sdk.CreditNoteService.IssueCreditNoteAsync(
    identification,
    parentCheqiReceiptId: "original-cheqi-receipt-id",
    creditNotePayload: new { documentNumber = "CN-001", totalAmount = -12.10m },
    cancellationToken: cancellationToken);
```

The payload can be a serializable object, `JsonElement`, or an already serialized JSON string. Amounts are not recalculated. Credit-note issuance requires owner devices and preserves the parent receipt reference.

Decrypt API or webhook receipts into a complete generated `ReceiptEnvelope`:

```csharp
var envelope = sdk.DecryptionService.DecryptReceipt(receiptDelivery, privateKey);
```

`DecryptCreditNoteInitiation` returns plaintext customer-return JSON; `DecryptCreditNoteInitiationRequest` decrypts, deserializes, and validates the current return contract. Both accept generated API request/response and webhook models. Private keys support PKCS#8 PEM or base64 DER; public keys support PEM, SPKI DER, and raw PKCS#1 DER.

Validated customer returns use `customerNote`, line items with `productId`, `quantity`, `reasonCode`, and `reasonDescription`, plus `refundPreference` and optional `refundBankAccount`. Bank-transfer refunds require an IBAN and account holder; other methods reject a supplied bank account.

## Stores and verification

`StoreService` provides create, list, active list, get, update, delete, activate, and deactivate operations. IDs are `Guid`, requests are generated `CreateStoreRequest`, and results are generated `StoreDTO`. Activation and deactivation return `Task` and accept no-content responses. Active-only filtering is performed by the server.

`VerificationService` retains the Java SDK’s canonical JSON format and SHA-256 hashing, UBL XML canonicalization, and cross-platform hash verification. Use `CheqiJson.CreateOptions()` whenever serializing generated models or persisted results to preserve wire enum values, explicit zero amounts, false booleans, optional field presence, plain-decimal money formatting, and ISO date offsets. Object and wire JSON hashing use the same serializer.

## Migration from 2.x

- Replace the trailing `accessToken` argument with `options: RequestOptions.WithAccessToken(accessToken)`.
- `SupplierClientId`, `SupplierClientSecret`, `SupplierCredentials(...)` and `ICheqiApiClient.GetAccessTokenAsync` are removed. Request per-Grant tokens with `OAuthService.RequestAccessTokenAsync` and store them in your integration.
- `InviteUsersAsync` and `GetStoresAsync` now take `resendPending` / `activeOnly` before `options` and `cancellationToken`.

## Migration from 1.x

.NET 8 or later is required. .NET 6 is no longer supported; its macOS AES-GCM implementation cannot run this encryption flow.

- Replace `ProcessCompleteReceiptAsync` and remote template generation with `IssueReceiptAsync` and generated `ReceiptPayload`.
- Replace credit-note template generation with `IssueCreditNoteAsync` and definitive caller-supplied values.
- Replace legacy encrypted receipt/credit-note DTOs with generated encrypted envelopes and deliveries.
- Replace template response merges and `ProcessEncryptedReceipt` with complete envelope decryption through `DecryptionService`.
- Replace `Models.Company` store models and string IDs with generated store models and `Guid` IDs.
- Company provisioning and server template endpoints were retired from the current API and are removed, matching Java.
- Replace old `lineItemId`, `quantityReturned`, and condition-based return fields with the current product, quantity, and reason contract.
- The SDK exception is named `CheqiSdkException`. Use async store operations instead of synchronous wrappers.
- Configure `ReceiptDownloadBaseUrl` for custom API endpoints that need offline fallback.

## Development

```sh
dotnet build cheqi-sdk.csproj --configuration Release
dotnet test tests/cheqi-sdk.Tests/cheqi-sdk.Tests.csproj --configuration Release
dotnet pack cheqi-sdk.csproj --configuration Release --no-build
```

The test suite simulates HTTP responses without platform credentials and covers routing, ambiguous acceptance, original-match recovery, encryption interoperability, downloads, customer-return validation, store wire models, DI, cancellation, and hashing.

To update the API contract, install OpenAPI Generator and Python's development dependencies, then run:

```sh
python3 -m pip install -r requirements-dev.txt
make fetch-spec ENV=prod
make generate
make spec-check ENV=prod
```

Model generation uses OpenAPI Generator 7.14.0 and runs `scripts/prepare-generated-models.py` to register generated JSON converters, preserve optional enum absence, normalize numeric and date serialization, and fix the model-only generator's submission-response union. Schema drift checks compare parsed YAML so mapping-order changes do not cause false failures.

MIT licensed. Support: support@cheqi.io.
