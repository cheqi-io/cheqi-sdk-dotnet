namespace Cheqi.Sdk.Matching;

/// <summary>
/// Represents a customer identifier used for matching.
/// 
/// Immutable value object containing an identifier type and value.
/// Use the builder pattern to create instances.
/// </summary>
public class CustomerIdentifier
{
    public IdentifierType Type { get; }
    public string Value { get; }

    private CustomerIdentifier(Builder builder)
    {
        Type = builder._type;
        Value = builder._value!;
    }

    /// <summary>
    /// Creates a new builder for CustomerIdentifier.
    /// </summary>
    /// <returns>A new builder instance</returns>
    public static Builder BuilderInstance() => new Builder();

    /// <summary>
    /// Builder for CustomerIdentifier.
    /// </summary>
    public class Builder
    {
        internal IdentifierType _type;
        internal string? _value;

        /// <summary>
        /// Sets the identifier type.
        /// </summary>
        /// <param name="type">The identifier type (PAR, PAN, IBAN, EMAIL)</param>
        /// <returns>This builder instance</returns>
        public Builder Type(IdentifierType type)
        {
            _type = type;
            return this;
        }

        /// <summary>
        /// Sets the identifier value.
        /// </summary>
        /// <param name="value">The identifier value</param>
        /// <returns>This builder instance</returns>
        public Builder Value(string value)
        {
            _value = value;
            return this;
        }

        /// <summary>
        /// Builds the CustomerIdentifier.
        /// </summary>
        /// <returns>A new CustomerIdentifier instance</returns>
        /// <exception cref="InvalidOperationException">if type or value is not set</exception>
        public CustomerIdentifier Build()
        {
            if (_type == default)
            {
                throw new InvalidOperationException("Identifier type must be set");
            }
            if (string.IsNullOrWhiteSpace(_value))
            {
                throw new InvalidOperationException("Identifier value must be set and non-empty");
            }
            return new CustomerIdentifier(this);
        }
    }
}
