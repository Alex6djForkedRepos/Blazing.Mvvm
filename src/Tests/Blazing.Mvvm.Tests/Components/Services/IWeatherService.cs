using Blazing.Mvvm.Tests.Components.Models;

namespace Blazing.Mvvm.Tests.Components.Services;

public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>?> GetForecastAsync(CancellationToken cancellationToken = default);
}
