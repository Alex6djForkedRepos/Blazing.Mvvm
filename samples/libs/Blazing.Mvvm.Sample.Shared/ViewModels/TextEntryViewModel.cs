using System.Text;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for handling ASCII text entry and conversion to and from hexadecimal.
/// </summary>
public sealed partial class TextEntryViewModel : RecipientViewModelBase<ConvertHexToAsciiMessage>, IRecipient<ResetHexAsciiInputsMessage>
{
    /// <summary>
    /// Gets or sets the ASCII text input.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendToHexConverterCommand))]
    private string? _asciiText;

    /// <inheritdoc />
    public override void Receive(ConvertHexToAsciiMessage message)
    {
        var asciiBuilder = new StringBuilder();

        for (int i = 0; i < message.HexToConvert.Length; i += 2)
        {
            string hs = message.HexToConvert.Substring(i, 2);
            uint decimalVal = Convert.ToUInt32(hs, 16);
            char character = Convert.ToChar(decimalVal);
            asciiBuilder.Append(character);
        }

        AsciiText = asciiBuilder.ToString();
    }

    /// <inheritdoc />
    public void Receive(ResetHexAsciiInputsMessage _)
        => AsciiText = string.Empty;

    /// <summary>
    /// Sends the current ASCII text to the hex converter via messenger.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSendToHexConverter))]
    private void SendToHexConverter()
        => Messenger.Send(new ConvertAsciiToHexMessage(AsciiText!));

    /// <summary>
    /// Determines whether the <see cref="SendToHexConverterCommand"/> can execute.
    /// </summary>
    /// <returns><c>true</c> if <see cref="AsciiText"/> is not null or whitespace; otherwise, <c>false</c>.</returns>
    private bool CanSendToHexConverter()
        => !string.IsNullOrWhiteSpace(AsciiText);
}
