using Undercut.Api.Auth;

namespace Undercut.Tests.Auth;

/// <summary>
/// Guards the guarantees of <see cref="RegistrationValidationResult"/>: a valid result always
/// has usable normalized values, a failed result always has errors, the errors can't be changed
/// after construction, and <c>ToString()</c> never prints user input.
/// Test names follow <c>Method_Scenario_ExpectedResult</c>.
/// </summary>
public class RegistrationValidationResultTests
{
    /// <summary>
    /// Returns a fresh two-field errors dictionary on every call, so tests never share state.
    /// The underlying type is a mutable <see cref="Dictionary{TKey, TValue}"/> on purpose: the
    /// aliasing test casts it back so it can change the original after construction.
    /// </summary>
    private static IReadOnlyDictionary<string, string[]> CreateErrors()
    {
        IReadOnlyDictionary<string, string[]> errors = new Dictionary<string, string[]>()
        {
            { "email", [ "Email is required" ] },
            { "displayName", [ "DisplayName is required" ] }
        };
        return errors;
    }

    /// <summary>A successful result is valid, has no errors, and carries both normalized values.</summary>
    [Fact]
    public void Success_WithValues_IsValidAndCarriesNormalizedValues()
    {
        RegistrationValidationResult result = RegistrationValidationResult.Success("alice@example.com", "alicedisplayname");

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Equal("alice@example.com", result.NormalizedEmail);
        Assert.Equal("alicedisplayname", result.NormalizedDisplayName);
    }

    /// <summary>
    /// A "valid" result must never be built without a usable email, or the endpoint could
    /// store a blank one. Uses <c>ThrowsAny</c> because null throws
    /// <see cref="ArgumentNullException"/> (a subtype), while blank text throws
    /// <see cref="ArgumentException"/>.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Success_NullOrBlankEmail_ThrowsArgumentException(string? email)
    {
        Assert.ThrowsAny<ArgumentException>(() => RegistrationValidationResult.Success(email!, "alicedisplayname"));
    }

    /// <summary>The same guarantee for the display name.</summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Success_NullOrBlankDisplayName_ThrowsArgumentException(string? displayName)
    {
        Assert.ThrowsAny<ArgumentException>(() => RegistrationValidationResult.Success("alice@example.com", displayName!));
    }

    /// <summary>A failed result is invalid, keeps its errors, and has null normalized values.</summary>
    [Fact]
    public void Failure_WithErrors_IsInvalidWithNullNormalizedValues()
    {
        var errors = CreateErrors();
        RegistrationValidationResult result = RegistrationValidationResult.Failure(errors);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(["Email is required"], result.Errors["email"]);
        Assert.Equal(["DisplayName is required"], result.Errors["displayName"]);
        Assert.Null(result.NormalizedEmail);
        Assert.Null(result.NormalizedDisplayName);
    }

    /// <summary>Null errors are a programming error and must be rejected.</summary>
    [Fact]
    public void Failure_NullErrors_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => RegistrationValidationResult.Failure(null!));
    }

    /// <summary>An empty errors dictionary would create a "failure" that reports itself as valid.</summary>
    [Fact]
    public void Failure_EmptyErrors_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => RegistrationValidationResult.Failure(new Dictionary<string, string[]>()));
    }

    /// <summary>
    /// The result must hold its own copy of the errors, so whoever built the original dictionary
    /// can't change a result after the fact.
    /// </summary>
    [Fact]
    public void Failure_OriginalDictionaryChangedAfterwards_ResultErrorsUnchanged()
    {
        var errors = CreateErrors();
        RegistrationValidationResult result = RegistrationValidationResult.Failure(errors);

        // Modify the original dictionary
        var mutableErrors = (Dictionary<string, string[]>)errors;
        mutableErrors["additionalField"] = ["Added additional error"];
        mutableErrors["email"] = ["Modified email error"];

        // Assert that the result's errors remain unchanged
        Assert.Equal(2, result.Errors.Count);
        Assert.Equal(["Email is required"], result.Errors["email"]);
        Assert.Equal(["DisplayName is required"], result.Errors["displayName"]);
        Assert.DoesNotContain("additionalField", result.Errors);
    }

    /// <summary>
    /// The errors are frozen: casting them back to a mutable dictionary and trying to change
    /// them must fail, so a failed result can't be turned into a "valid" one.
    /// </summary>
    [Fact]
    public void Failure_ErrorsCastToMutableDictionary_ClearThrowsNotSupportedException()
    {
        var errors = CreateErrors();
        RegistrationValidationResult result = RegistrationValidationResult.Failure(errors);

        Assert.Throws<NotSupportedException>(() =>
        {
            var mutableErrors = (IDictionary<string, string[]>)result.Errors;
            mutableErrors.Clear();
        });
    }

    /// <summary>
    /// A valid result's log output is fixed text and never contains the email or display name.
    /// Distinctive fake values make sure a leak can't hide behind a common word.
    /// </summary>
    [Fact]
    public void ToString_Valid_DoesNotContainEmailOrDisplayName()
    {
        string email = "zebra-quartz@example.com";
        string displayName = "zebra-quartz-display-name";

        RegistrationValidationResult result = RegistrationValidationResult.Success(email, displayName);
        string resultString = result.ToString();

        Assert.DoesNotContain(email, resultString);
        Assert.DoesNotContain(displayName, resultString);
        Assert.Equal("Valid Result", resultString);
    }

    /// <summary>A failed result's log output shows only the error count, never the messages.</summary>
    [Fact]
    public void ToString_Invalid_DoesNotContainErrorText()
    {
        IReadOnlyDictionary<string, string[]> errors = new Dictionary<string, string[]>()
        {
            { "email", [ "mango-test-message" ] }
        };

        RegistrationValidationResult result = RegistrationValidationResult.Failure(errors);
        string resultString = result.ToString();

        Assert.DoesNotContain("mango-test-message", resultString);
        Assert.Equal("Invalid Result. Errors: 1", resultString);
    }
}