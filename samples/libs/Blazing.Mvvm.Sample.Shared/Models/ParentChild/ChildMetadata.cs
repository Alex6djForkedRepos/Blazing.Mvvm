namespace Blazing.Mvvm.Sample.Shared.Models.ParentChild;

/// <summary>
/// Represents metadata for a child component, including its parameters.
/// </summary>
public class ChildMetadata
{
    /// <summary>
    /// Gets or sets the parameters to be passed to the child component.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = new();
}
