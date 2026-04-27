using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blazing.Mvvm.Sample.Shared.ViewModels.RelayCommands;

/// <summary>
/// ViewModel demonstrating an async command with enable/disable logic for Blazor UI.
/// </summary>
public partial class Example1AsyncViewModel : ViewModelBase
{
    /// <summary>
    /// Indicates whether the command is disabled.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ClickedCommand))]
    private bool _isDisabled;

    /// <summary>
    /// Message to display in the UI.
    /// </summary>
    [ObservableProperty]
    private string _message = "No message set";

    /// <summary>
    /// Async command executed when the button is clicked.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [RelayCommand(CanExecute = nameof(CanClick))]
    private async Task ClickedAsync()
    {
        await Task.Delay(2000);
        Message = $"Async clicked at {DateTime.Now:T}";
    }

    /// <summary>
    /// Determines if the command can execute.
    /// </summary>
    /// <returns>True if not disabled; otherwise, false.</returns>
    private bool CanClick() => !IsDisabled;
}
