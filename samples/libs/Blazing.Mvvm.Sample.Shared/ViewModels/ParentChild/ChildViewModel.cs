using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Models.ParentChild;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Sample.Shared.ViewModels.ParentChild;

/// <summary>
/// ViewModel for a child component, providing text and a close command to notify the parent.
/// </summary>]
[ViewModelDefinition(Lifetime = ServiceLifetime.Transient)]
public partial class ChildViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the text associated with the child view.
    /// </summary>
    [ObservableProperty]
    private string _text = "Child";

    /// <summary>
    /// Sends a message to the parent to close this child view.
    /// </summary>
    [RelayCommand]
    public virtual void Close()
        // tell the parent to close us...
        => WeakReferenceMessenger.Default.Send(new ChildMessage(Text));
}
