using System.Collections.Frozen;

namespace Undercut.Api.Auth;

/// <summary>
/// Outcome of validating a registration request. Immutable: create one with
/// <see cref="Success"/> or <see cref="Failure"/>.
/// </summary>
/// <remarks>
/// A valid result always carries both normalized values and no errors. A failed result always
/// carries at least one error and no normalized values. The password is deliberately not part
/// of this type, and <see cref="ToString"/> never prints user input.
/// </remarks>
public sealed record RegistrationValidationResult
{
    /// <summary>
    /// Validation errors keyed by field name (the keys are sent to the client, so they use the
    /// same camelCase names as the request JSON). Empty when valid. This is a frozen copy, so
    /// neither the caller nor the original builder can change it after construction.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>The trimmed, lowercased email to store. Null when validation failed.</summary>
    public string? NormalizedEmail { get; }

    /// <summary>The trimmed, Unicode-normalized display name to store. Null when validation failed.</summary>
    public string? NormalizedDisplayName { get; }

    /// <summary>
    /// True when there are no errors. Derived from <see cref="Errors"/> rather than stored,
    /// so the two can never disagree.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Private so the only ways to create a result are <see cref="Success"/> and <see cref="Failure"/>.
    /// </summary>
    private RegistrationValidationResult(string? normalizedEmail, string? normalizedDisplayName, IReadOnlyDictionary<string, string[]> errors)
    {
        NormalizedEmail = normalizedEmail;
        NormalizedDisplayName = normalizedDisplayName;
        Errors = errors.ToFrozenDictionary();
    }

    /// <summary>Creates a valid result carrying the values to store.</summary>
    /// <param name="normalizedEmail">The normalized email.</param>
    /// <param name="normalizedDisplayName">The normalized display name.</param>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    /// <exception cref="ArgumentException">An argument is empty or whitespace.</exception>
    public static RegistrationValidationResult Success(string normalizedEmail, string normalizedDisplayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail, nameof(normalizedEmail));
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedDisplayName, nameof(normalizedDisplayName));
        return new(normalizedEmail, normalizedDisplayName, new Dictionary<string, string[]>());
    }

    /// <summary>Creates a failed result from the collected errors.</summary>
    /// <param name="errors">
    /// Messages grouped by field name. Messages must be fixed text and must never contain
    /// the submitted values.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="errors"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="errors"/> is empty, which would make a failure report itself as valid.</exception>
    public static RegistrationValidationResult Failure(IReadOnlyDictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));
        return new(null, null, errors is { Count: > 0 } ? errors : throw new ArgumentException("Errors cannot be empty.", nameof(errors)));
    }

    /// <summary>
    /// Log-safe description: validity and error count only. Never includes the email,
    /// the display name, or any error message.
    /// </summary>
    public override string ToString() => IsValid ? "Valid Result" : $"Invalid Result. Errors: {Errors.Count}";
}