# Java and Python capability mapping

The Java and Python SDKs are the functional reference. The .NET SDK exposes the
following service operations through `CheqiSdk`; asynchronous operations also accept
per-request `RequestOptions` and `CancellationToken`. Generated DTOs follow the
production OpenAPI contract. Language-specific overloads and configuration builders
need not have identical names.

| Capability in Java/Python | .NET public API |
| --- | --- |
| Create, list, filter active, get, update, delete, activate and deactivate stores | `StoreService.CreateStoreAsync`, `GetStoresAsync(activeOnly)`, `GetActiveStoresAsync`, `GetStoreAsync`, `UpdateStoreAsync`, `DeleteStoreAsync`, `ActivateStoreAsync`, `DeactivateStoreAsync` |
| Invite company users, including resend | `CompanyService.InviteUsersAsync` |
| List/register destinations, poll pending receipts, acknowledge and deactivate | `CompanyService.ListReceiptDestinationsAsync`, `RegisterReceiptDestinationAsync`, `GetPendingReceiptsAsync`, `AcknowledgeReceiptsAsync`, `DeactivateReceiptDestinationAsync` |
| Change a webhook subscription URL | `WebhookService.UpdateSubscriptionUrlAsync` |
| Obtain a client-credentials token | `OAuthService.RequestAccessTokenAsync` |
| Resolve a customer/device route | `MatchingService.MatchCustomerAsync` |
| Validate caller-supplied receipt values | `ReceiptService.Validate`; standalone `ReceiptPayloadValidator.Validate`, `ValidateJson`, `ValidateOrThrow`; `CheqiSdkException.ValidationReport` |
| Issue, recover by original match, generate and submit encrypted receipts | `ReceiptService.IssueReceiptAsync`, `GetMatchAsync`, `ResumeReceiptAsync`, `GenerateEncryptedReceiptEnvelope`, `SubmitEncryptedReceiptAsync` |
| Issue downloads, upload a persisted fallback and complete a fallback envelope | `ReceiptService.IssueDownloadReceiptAsync`, `UploadPreparedDownloadAsync`, `CompleteDownloadFallbackAsync` |
| Python's legacy version 1 browser document helper | `ReceiptService.BuildDownloadCheqiDocument` |
| Issue a credit note and Python's low-level encrypted submission helper | `CreditNoteService.IssueCreditNoteAsync`, `SubmitEncryptedCreditNoteAsync` |
| Device encryption and receipt/return-request decryption | `EncryptionService.EncryptForDevice`, `EncryptReceiptForRecipient`, `EncryptCreditNoteForRecipient`; `DecryptionService.DecryptReceipt`, `DecryptCreditNoteInitiation`, `DecryptCreditNoteInitiationRequest` |
| Download URL/key generation, parsing and envelope encryption/decryption | `DownloadService.GenerateDownloadLink`, `BuildDownloadUrl`, `ParseDownloadUrl`, `EncryptDownloadEnvelope`, `DecryptDownloadEnvelope` |
| Canonical JSON/XML hashes and Python's verification helpers | `VerificationService.CanonicalizeCheqiReceipt`, `CalculateCheqiReceiptHash`, `CanonicalizeUbl`, `CalculateUblHash`, `CanonicalizeAndHashJson`, `CanonicalizeAndHashXml`, `VerifyJsonHash`, `VerifyXmlHash` |

Store, company, webhook, OAuth, encryption and receipt-recovery functionality was
already exposed. This change adds the shared validation engine, credit-note
submission convenience method, legacy document builder and hash-verification
helpers, and aligns `DownloadService`'s Test environment with Python.

The validator bundle and compressed golden reports are copied unchanged from the
Python reference. `Validation/Resources/manifest.json` records the engine version,
clean source commit, target, ABI and SHA-256 digest; the digest and provenance are
checked before module compilation. The tests compare every shared report, exercise
concurrent calls, reject corrupted bundles, stop invalid receipts before HTTP, and
verify that validation leaves numeric wire serialization and encrypted plaintext
unchanged.
