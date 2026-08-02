/// <summary>
/// Contains message types for hex and ASCII conversion operations.
/// </summary>
namespace Blazing.Mvvm.Sample.Shared.Models;

/// <summary>
/// Message to request conversion from hexadecimal to ASCII.
/// </summary>
/// <param name="HexToConvert">The hexadecimal string to convert.</param>
public record ConvertHexToAsciiMessage(string HexToConvert);

/// <summary>
/// Message to request conversion from ASCII to hexadecimal.
/// </summary>
/// <param name="AsciiToConvert">The ASCII string to convert.</param>
public record ConvertAsciiToHexMessage(string AsciiToConvert);

/// <summary>
/// Message to request resetting the hex and ASCII input fields.
/// </summary>
public record ResetHexAsciiInputsMessage;
