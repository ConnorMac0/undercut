using System.Text.Json.Serialization;
using Undercut.Api.Common;

namespace Undercut.Api.Auth;

/// <summary>
/// Body of <c>POST /auth/register</c>. Properties are nullable because the JSON deserializer
/// can produce null whatever the declared type; the endpoint must validate every field.
/// </summary>
/// <param name="Email">The address to register. Normalized (trimmed, lowercased) before storage.</param>
/// <param name="Password">
/// Plain-text password, accepted on input only. Excluded from serialization
/// (<see cref="JsonIgnoreCondition.WhenWriting"/>) so it cannot leak through JSON output.
/// </param>
/// <param name="DisplayName">Name shown to other league members.</param>
public sealed record RegisterRequest(string? Email, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)] string? Password, string? DisplayName)
{
    /// <summary>
    /// Log-safe description: masked email and display name. Never includes the password.
    /// </summary>
    public override string ToString()
    {
        return $"RegisterRequest(Email: {Email.MaskEmail()}, DisplayName: {DisplayName})";
    }
}

/// <summary>
/// Body of <c>POST /auth/login</c>. Properties are nullable for the same reason as
/// <see cref="RegisterRequest"/>.
/// </summary>
/// <param name="Email">The registered address. Normalized the same way as at registration.</param>
/// <param name="Password">
/// Plain-text password, accepted on input only. Excluded from serialization
/// (<see cref="JsonIgnoreCondition.WhenWriting"/>) so it cannot leak through JSON output.
/// </param>
public sealed record LoginRequest(string? Email, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)] string? Password)
{
    /// <summary>
    /// Log-safe description: masked email only. Never includes the password.
    /// </summary>
    public override string ToString()
    {
        return $"LoginRequest(Email: {Email.MaskEmail()})";
    }
}

/// <summary>Response to a successful registration.</summary>
/// <param name="Id">Identifier of the newly created user. Nothing else is returned.</param>
public sealed record RegisterResponse(Guid Id);

/// <summary>
/// Response to a successful login. Shaped after OAuth 2.0 (RFC 6749) semantics; JSON property
/// names are camelCase (<c>accessToken</c>, <c>expiresIn</c>), not the RFC's snake_case.
/// </summary>
/// <param name="TokenType">Authorization scheme for the <c>Authorization</c> header. Always "Bearer".</param>
/// <param name="AccessToken">The signed JWT. Send it as <c>Authorization: Bearer {AccessToken}</c>.</param>
/// <param name="ExpiresIn">
/// Token lifetime in <b>seconds</b>, counted from the moment the response is issued.
/// Configuration stores minutes (<c>Jwt:ExpiryMinutes</c>), so convert before building this value.
/// </param>
public sealed record LoginResponse(string TokenType, string AccessToken, int ExpiresIn);

/// <summary>
/// Response for <c>GET /auth/me</c>: the authenticated caller's own details.
/// The email is safe here only because the caller is receiving their own data;
/// do not reuse this shape for endpoints that return other users.
/// </summary>
/// <param name="Id">The caller's user identifier.</param>
/// <param name="Email">The caller's own email address.</param>
/// <param name="DisplayName">The caller's display name.</param>
public sealed record MeResponse(Guid Id, string Email, string DisplayName);