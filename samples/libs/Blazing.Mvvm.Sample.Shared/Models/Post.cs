
/// <summary>
/// Represents a post created by a user.
/// </summary>
namespace Blazing.Mvvm.Sample.Shared.Models;

/// <summary>
/// Represents a post created by a user.
/// </summary>
public record Post
{
    /// <summary>
    /// Gets the unique identifier for the post.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the identifier of the user who created the post.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the title of the post.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the content of the post.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets the preview text for the post.
    /// </summary>
    public required string Preview { get; init; }

    /// <summary>
    /// Gets the date and time when the post was created (in UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
