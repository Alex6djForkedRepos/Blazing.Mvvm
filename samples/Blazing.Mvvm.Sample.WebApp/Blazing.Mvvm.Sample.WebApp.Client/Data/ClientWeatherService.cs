using System.Net.Http.Json;
using Blazing.Mvvm.Sample.Shared.Data;
using Blazing.Mvvm.Sample.Shared.Models;

namespace Blazing.Mvvm.Sample.WebApp.Client.Data;

/// <summary>
/// Client-side implementation of the weather service that calls the server API.
/// </summary>
public class ClientWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientWeatherService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public ClientWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public Task<IEnumerable<WeatherForecast>?> GetForecastAsync(CancellationToken cancellationToken = default)
        => _httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("/api/weatherforecast", cancellationToken);
}
