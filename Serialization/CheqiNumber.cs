using System.Globalization;

namespace Cheqi.Sdk.Serialization;

internal static class CheqiNumber
{
    // Expand round-trip digits into plain decimal notation, as Java's BigDecimal does.
    internal static string PlainDouble(double value, bool retainFloatingScale)
    {
        if (!double.IsFinite(value)) throw new System.Text.Json.JsonException("Non-finite numbers are not valid JSON");
        if (value == 0) return retainFloatingScale ? "0.0" : "0";
        var negative = value < 0;
        var text = Math.Abs(value).ToString("R", CultureInfo.InvariantCulture);
        var parts = text.Split('E', 'e');
        var exponent = parts.Length == 2 ? int.Parse(parts[1], CultureInfo.InvariantCulture) : 0;
        var dot = parts[0].IndexOf('.');
        var point = (dot < 0 ? parts[0].Length : dot) + exponent;
        var digits = parts[0].Replace(".", "");
        if (retainFloatingScale)
        {
            var scientific = Math.Abs(value) < 0.001 || Math.Abs(value) >= 10000000;
            if (scientific)
            {
                // Java Double.toString always gives a fractional digit in its mantissa.
                var leadingZeros = digits.TakeWhile(c => c == '0').Count();
                digits = digits[leadingZeros..];
                point -= leadingZeros;
                if (digits.Length == 1) digits += "0";
            }
            else if (dot < 0 && parts.Length == 1) digits += "0";
        }
        var plain = point <= 0 ? "0." + new string('0', -point) + digits
            : point >= digits.Length ? digits + new string('0', point - digits.Length)
            : digits[..point] + "." + digits[point..];
        if (!retainFloatingScale && plain.Contains('.')) plain = plain.TrimEnd('0').TrimEnd('.');
        return (negative ? "-" : "") + plain;
    }
}
