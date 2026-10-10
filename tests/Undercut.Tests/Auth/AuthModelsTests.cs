using System.Text.Json;
using Undercut.Api.Auth;

namespace Undercut.Tests.Auth;

/// <summary>
/// Guards the request models against leaking passwords and against breaking login.
/// The password must be readable from incoming JSON but never appear in JSON output
/// or in <c>ToString()</c> output (which is what ends up in logs).
/// Test names follow <c>Type_Method_Scenario_ExpectedResult</c>.
/// </summary>
public class AuthModelsTests
{
    /// <summary>Matches the options ASP.NET Core uses for API JSON (camelCase, case-insensitive).</summary>
    private static readonly JsonSerializerOptions WebOptions = new(JsonSerializerDefaults.Web);

    // ---------- RegisterRequest ----------

    /// <summary>The log-safe description masks the email and never contains the password.</summary>
    [Fact]
    public void RegisterRequest_ToString_WithPassword_MasksEmailAndOmitsPassword()
    {
        var request = new RegisterRequest("alice@example.com", "password123", "Alice");

        var result = request.ToString();

        Assert.Contains("*****@example.com", result);
        Assert.Contains("Alice", result);
        Assert.DoesNotContain("password123", result);
    }

    /// <summary>Serializing to JSON must not emit the password.</summary>
    [Fact]
    public void RegisterRequest_Serialize_WithPassword_OmitsPassword()
    {
        var request = new RegisterRequest("alice@example.com", "password123", "Alice");

        var result = JsonSerializer.Serialize(request, WebOptions);

        Assert.Contains("alice@example.com", result);
        Assert.Contains("Alice", result);
        Assert.DoesNotContain("password123", result);
        using var doc = JsonDocument.Parse(result);
        Assert.False(doc.RootElement.TryGetProperty("password", out _));
    }

    /// <summary>
    /// The password must still be read from an incoming request body, or registration
    /// would receive a null password for every client.
    /// </summary>
    [Fact]
    public void RegisterRequest_Deserialize_PopulatesPassword()
    {
        var json = "{\"email\":\"alice@example.com\",\"password\":\"password123\",\"displayName\":\"Alice\"}";

        var result = JsonSerializer.Deserialize<RegisterRequest>(json, WebOptions);

        Assert.Equal("alice@example.com", result!.Email);
        Assert.Equal("Alice", result!.DisplayName);
        Assert.Equal("password123", result!.Password);
    }

    // ---------- LoginRequest ----------

    /// <summary>The log-safe description masks the email and never contains the password.</summary>
    [Fact]
    public void LoginRequest_ToString_WithPassword_MasksEmailAndOmitsPassword()
    {
        var request = new LoginRequest("alice@example.com", "password123");

        var result = request.ToString();

        Assert.Contains("*****@example.com", result);
        Assert.DoesNotContain("password123", result);
    }

    /// <summary>Serializing to JSON must not emit the password.</summary>
    [Fact]
    public void LoginRequest_Serialize_WithPassword_OmitsPassword()
    {
        var request = new LoginRequest("alice@example.com", "password123");

        var result = JsonSerializer.Serialize(request, WebOptions);

        Assert.Contains("alice@example.com", result);
        Assert.DoesNotContain("password123", result);
        using var doc = JsonDocument.Parse(result);
        Assert.False(doc.RootElement.TryGetProperty("password", out _));
    }

    /// <summary>
    /// The password must still be read from an incoming request body, or login
    /// would receive a null password for every client.
    /// </summary>
    [Fact]
    public void LoginRequest_Deserialize_PopulatesPassword()
    {
        var json = "{\"email\":\"alice@example.com\",\"password\":\"password123\"}";

        var result = JsonSerializer.Deserialize<LoginRequest>(json, WebOptions);

        Assert.Equal("alice@example.com", result!.Email);
        Assert.Equal("password123", result!.Password);
    }
}