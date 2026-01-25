using System.Text;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Configuration class for encryption settings and key management.
///
/// This class manages:
/// - Encryption algorithm parameters
/// - Salt values for deterministic matching
/// - Key derivation settings
/// - Security parameters
/// </summary>
public class EncryptionConfig
{
    public byte[] MatchingSalt { get; }
    public string Environment { get; }
    public bool StrictMode { get; }

    private EncryptionConfig(Builder builder)
    {
        MatchingSalt = builder._matchingSalt ?? throw new ArgumentNullException(nameof(MatchingSalt));
        Environment = builder._environment;
        StrictMode = builder._strictMode;
    }

    public static Builder BuilderInstance() => new Builder();

    public byte[] GetMatchingSalt()
    {
        // Return copy for security
        return (byte[])MatchingSalt.Clone();
    }

    public class Builder
    {
        internal byte[]? _matchingSalt;
        internal string _environment = "production";
        internal bool _strictMode = true;

        public Builder MatchingSalt(string salt)
        {
            _matchingSalt = Encoding.UTF8.GetBytes(salt);
            return this;
        }

        public Builder MatchingSalt(byte[] salt)
        {
            _matchingSalt = (byte[])salt.Clone();
            return this;
        }

        public Builder Environment(string environment)
        {
            _environment = environment;
            return this;
        }

        public Builder StrictMode(bool strictMode)
        {
            _strictMode = strictMode;
            return this;
        }

        public EncryptionConfig Build()
        {
            if (_matchingSalt == null)
            {
                // Generate a random salt if none provided (for testing only)
                _matchingSalt = new byte[32];
                using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
                rng.GetBytes(_matchingSalt);
            }
            return new EncryptionConfig(this);
        }
    }
}
