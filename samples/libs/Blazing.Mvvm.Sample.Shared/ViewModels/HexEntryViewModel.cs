using System.Text;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for handling hexadecimal text entry and conversion to and from ASCII.
/// </summary>
public sealed partial class HexEntryViewModel : RecipientViewModelBase<ConvertAsciiToHexMessage>, IRecipient<ResetHexAsciiInputsMessage>
{
    /// <summary>
    /// Gets or sets the hexadecimal text input.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendToAsciiConverterCommand))]
    private string? _hexText;

    /// <summary>
    /// Receives a message to convert ASCII text to hexadecimal and updates <see cref="HexText"/>.
    /// </summary>
    /// <param name="message">The message containing the ASCII text to convert.</param>
    public override void Receive(ConvertAsciiToHexMessage message)
    {
        char[] charArray = message.AsciiToConvert.ToCharArray();
        var hexOutput = new StringBuilder();
        foreach (char @char in charArray)
        {
            hexOutput.AppendFormat("{0:X}", Convert.ToInt32(@char));
        }

        HexText = hexOutput.ToString();
    }

    /// <summary>
    /// Receives a message to reset the hex and ASCII input fields.
    /// </summary>
    /// <param name="_">The reset message (unused).</param>
    public void Receive(ResetHexAsciiInputsMessage _)
        => HexText = string.Empty;

    /// <summary>
    /// Sends the current hexadecimal text to the ASCII converter via messenger.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSendToAsciiConverter))]
    private void SendToAsciiConverter()
        => Messenger.Send(new ConvertHexToAsciiMessage(HexText!));

    /// <summary>
    /// Determines whether the <see cref="SendToAsciiConverterCommand"/> can execute.
    /// </summary>
    /// <returns><c>true</c> if <see cref="HexText"/> is not null or whitespace; otherwise, <c>false</c>.</returns>
    private bool CanSendToAsciiConverter()
        => !string.IsNullOrWhiteSpace(HexText);
}
