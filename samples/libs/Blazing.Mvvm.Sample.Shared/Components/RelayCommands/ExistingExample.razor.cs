using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.RelayCommands;

/// <summary>
/// Code-behind for the ExistingExample component. Demonstrates a simple synchronous button example.
/// </summary>
public partial class ExistingExample : ComponentBase
{
    /// <summary>
    /// Message to display in the UI.
    /// </summary>
    public string Message { get; set; } = "No message set";

    /// <summary>
    /// Indicates whether the button is disabled.
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// Handles the button click event and updates the message.
    /// </summary>
    private void OnClick()
    {
        Message = "Button clicked at " + DateTime.Now.ToLongTimeString();
    }
}
