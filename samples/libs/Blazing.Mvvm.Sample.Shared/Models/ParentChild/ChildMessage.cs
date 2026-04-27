namespace Blazing.Mvvm.Sample.Shared.Models.ParentChild;

/// <summary>
/// Represents a message sent from a child component or view model.
/// </summary>
/// <param name="message">The message content.</param>
public record ChildMessage(string message);
