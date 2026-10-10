namespace Undercut.Api.Common;

public static class EmailExtensions
{
    private const string MissingPlaceholder = "Invalid or Missing Email";
    private const string FormatPlaceholder = "Invalid Email Format";
    private const string Mask = "*****";

    /// <summary>
    /// Returns a log-safe version of an email: first character, a fixed mask, and the domain.
    /// Never echoes input that doesn't look like an email. Never throws.
    /// </summary>
    public static string MaskEmail(this string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return MissingPlaceholder;

        // Validate the shape before reading any part of it.
        var parts = email.Trim().Split('@');
        if (parts.Length != 2 || parts[0].Length == 0 || !IsPlausibleDomain(parts[1]))
            return FormatPlaceholder;

        var local = parts[0];
        var domain = parts[1];

        // A one-character local part would be fully revealed by showing its first character.
        return local.Length == 1
            ? $"{Mask}@{domain}"
            : $"{local[0]}{Mask}@{domain}";
    }

    // Conservative shape check, not email validation: "is this safe to print?"
    private static bool IsPlausibleDomain(string domain)
    {
        if (domain.Length is 0 or > 253 || !domain.Contains('.') || domain.Contains(".."))
            return false;

        if (domain.StartsWith('.') || domain.EndsWith('.'))
            return false;

        foreach (var c in domain)
        {
            if (!(char.IsAsciiLetterOrDigit(c) || c is '.' or '-'))
                return false;
        }

        return true;
    }
}