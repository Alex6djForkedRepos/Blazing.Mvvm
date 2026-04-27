using CommunityToolkit.Mvvm.ComponentModel;


/// <summary>
/// Represents a weather forecast with date, temperature, and summary information.
/// </summary>
namespace Blazing.Mvvm.Sample.Shared.Models;

/// <summary>
/// Represents a weather forecast with date, temperature, and summary information.
/// </summary>
public partial class WeatherForecast : ObservableObject
{
    /// <summary>
    /// Gets or sets the date of the forecast.
    /// </summary>
    [ObservableProperty]
    private DateTime _date;

    /// <summary>
    /// Gets or sets the temperature in Celsius.
    /// </summary>
    [ObservableProperty]
    private int _temperatureC;

    /// <summary>
    /// Updates the Fahrenheit temperature when the Celsius value changes.
    /// </summary>
    /// <param name="value">The new Celsius temperature value.</param>
    partial void OnTemperatureCChanged(int value)
    {
        TemperatureF = 32 + (int)(value / 0.5556);
    }

    /// <summary>
    /// Gets or sets the summary of the weather forecast.
    /// </summary>
    [ObservableProperty]
    private string? _summary;

    /// <summary>
    /// Gets or sets the temperature in Fahrenheit.
    /// </summary>
    [ObservableProperty]
    private int _temperatureF;
}
