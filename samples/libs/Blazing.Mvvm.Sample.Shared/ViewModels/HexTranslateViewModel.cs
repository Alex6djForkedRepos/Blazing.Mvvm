using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for translating and resetting hex and ASCII input fields via messaging.
/// </summary>
[ViewModelDefinition(Key = nameof(HexTranslateViewModel))]
public partial class HexTranslateViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HexTranslateViewModel"/> class.
    /// </summary>
    /// <param name="messenger">The messenger used to send messages to child components.</param>
    public HexTranslateViewModel(IMessenger messenger)
    {
        _messenger = messenger;
    }

    /// <summary>
    /// Sends a message to reset the hex and ASCII input fields in child components.
    /// </summary>
    [RelayCommand]
    public void ResetChildInputs()
        => _messenger.Send(new ResetHexAsciiInputsMessage());
}
