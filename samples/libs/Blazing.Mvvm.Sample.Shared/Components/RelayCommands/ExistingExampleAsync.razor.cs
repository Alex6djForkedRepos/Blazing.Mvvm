using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.RelayCommands;

/// <summary>
/// Code-behind for the ExistingExampleAsync component. Demonstrates an async button example with busy state.
/// </summary>
public partial class ExistingExampleAsync : ComponentBase
{
    /// <summary>
    /// Message to display in the UI.
    /// </summary>
    public string Message { get; set; } = "No message set";

    /// <summary>
    /// Indicates whether the button is disabled.
    /// </summary>
    public bool IsChecked { get; set; }

    private bool _isBusy;

    /// <summary>
    /// Handles the async button click event, disables the button while running, and updates the message.
    /// </summary>
    private async Task OnClickAsync()
    {
        if (_isBusy) return;
        _isBusy = true;
        try
        {
            await Task.Delay(2000); // Simulate async work
            Message = "Async button clicked at " + DateTime.Now.ToLongTimeString();
        }
        finally
        {
            _isBusy = false;
        }
    }
}
