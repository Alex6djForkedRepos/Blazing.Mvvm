using Blazing.Mvvm.Sample.Shared.Models;

namespace Blazing.Mvvm.Sample.Shared.Data;

/// <summary>
/// Service interface for weather forecast operations.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Gets the weather forecast asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of weather forecasts.</returns>
    Task<IEnumerable<WeatherForecast>?> GetForecastAsync(CancellationToken cancellationToken = default);
}
