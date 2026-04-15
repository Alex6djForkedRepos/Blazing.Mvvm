
/// <summary>
/// Represents a user in the system.
/// </summary>
namespace Blazing.Mvvm.Sample.Shared.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public record User
{
    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the email address of the user.
    /// </summary>
    public required string Email { get; init; }
}
