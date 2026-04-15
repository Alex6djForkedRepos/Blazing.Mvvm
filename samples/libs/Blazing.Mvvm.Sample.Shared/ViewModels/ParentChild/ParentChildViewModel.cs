using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Extensions;
using Blazing.Mvvm.Sample.Shared.Models.ParentChild;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Sample.Shared.ViewModels.ParentChild;

/// <summary>
/// ViewModel for demonstrating parent-child messaging and dynamic child component management.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public partial class ParentChildViewModel : RecipientViewModelBase<ChildMessage>
{
    /// <summary>
    /// Tracks the number of child components added.
    /// </summary>
    private int _count;

    /// <summary>
    /// Stores metadata for each child component, keyed by name.
    /// </summary>
    public readonly Dictionary<string, ChildMetadata> Components = new();

    /// <summary>
    /// Gets or sets the display text for the parent view.
    /// </summary>
    [ObservableProperty]
    private string? _text = "Welcome to the Parent-Child messaging demo.";

    /// <summary>
    /// Adds a new child component with incremented name and default parameters.
    /// </summary>
    [RelayCommand]
    public virtual void AddChild()
        => Components.AddChildComponent($"Child #{++_count}");

    /// <summary>
    /// Returns the set of child component keys sorted numerically by their index.
    /// </summary>
    /// <returns>An enumerable of sorted child component keys.</returns>
    public IEnumerable<string> GetSortedKeys()
        => Components.Keys
            .Select(x => x.Split('#')[1])
            .OrderBy(int.Parse)
            .Select(index => $"Child #{index}");

    /// <summary>
    /// Handles inbound messages from child components, removing the corresponding child and notifying state change.
    /// </summary>
    /// <param name="child">The message from the child component.</param>
    public override void Receive(ChildMessage child)
    {
        Components.Remove(child.message);
        NotifyStateChanged();
    }
}
