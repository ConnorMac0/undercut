using Undercut.Api.Common;

namespace Undercut.Tests.Common;

public class EmailExtensionsTests
{
    private const string MissingPlaceholder = "Invalid or Missing Email";
    private const string FormatPlaceholder = "Invalid Email Format";

    // Valid addresses reveal only the first character of the local part, a fixed run of
    // stars (so the length isn't leaked), and the domain.
    [Theory]
    [InlineData("alice@example.com", "a*****@example.com")]
    [InlineData("bob@corp.example.org", "b*****@corp.example.org")]
    [InlineData("ab@example.com", "a*****@example.com")]
    [InlineData("  alice@example.com  ", "a*****@example.com")] // surrounding whitespace is trimmed first
    public void MaskEmail_ValidAddress_RevealsOnlyFirstCharacterAndDomain(string email, string expected)
    {
        Assert.Equal(expected, email.MaskEmail());
    }

    // The property that matters most: the real local part must never appear in the output.
    [Theory]
    [InlineData("alice@example.com", "alice")]
    [InlineData("bob@corp.example.org", "bob")]
    [InlineData("ab@example.com", "ab")]
    public void MaskEmail_ValidAddress_NeverContainsFullLocalPart(string email, string localPart)
    {
        Assert.DoesNotContain(localPart, email.MaskEmail());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void MaskEmail_NullBlankOrNoAtSign_ReturnsMissingPlaceholder(string? email)
    {
        Assert.Equal(MissingPlaceholder, email.MaskEmail());
    }

    [Theory]
    [InlineData("@example.com")]       // nothing before the @
    [InlineData("alice@")]             // nothing after the @
    [InlineData("a@b@c.com")]          // more than one @
    [InlineData("alice@example")]      // domain has no dot
    [InlineData("alice@exam!ple.com")] // character that cannot appear in a domain
    [InlineData("alice@example.")]     // trailing dot
    [InlineData("alice@.example.com")] // leading dot
    public void MaskEmail_MalformedAddress_ReturnsFormatPlaceholder(string email)
    {
        Assert.Equal(FormatPlaceholder, email.MaskEmail());
    }

    // Regression guard: someone typing a password into the email box must not be echoed to logs.
    [Theory]
    [InlineData("p@ssw0rd!")]
    [InlineData("Tr0ub4dor&3@")]
    [InlineData("hunter2")]
    public void MaskEmail_PasswordTypedIntoEmailField_IsNotEchoed(string input)
    {
        var result = input.MaskEmail();

        Assert.DoesNotContain(input, result);
        Assert.StartsWith("Invalid", result, StringComparison.Ordinal);
    }

    [Fact]
    public void MaskEmail_SingleCharacterLocalPart_RevealsNothingOfIt()
    {
        Assert.Equal("*****@example.com", "a@example.com".MaskEmail());
    }
}