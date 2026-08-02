using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Sample.Shared.Data;
using Blazing.Mvvm.Sample.Shared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for fetching and persisting weather forecast data, supporting state persistence and cancellation.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class FetchDataViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<FetchDataViewModel> _logger;
    private readonly PersistentComponentState? _state;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private PersistingComponentStateSubscription _persistingSubscription;

    /// <summary>
    /// Gets or sets the collection of weather forecasts.
    /// </summary>
    [ObservableProperty]
    private IEnumerable<WeatherForecast>? _weatherForecasts;

    /// <summary>
    /// Initializes a new instance of the <see cref="FetchDataViewModel"/> class.
    /// </summary>
    /// <param name="weatherService">The weather service used to fetch forecast data.</param>
    /// <param name="logger">The logger instance for logging events.</param>
    /// <param name="state">The persistent component state for state management (optional).</param>
    public FetchDataViewModel(
        IWeatherService weatherService, 
        ILogger<FetchDataViewModel> logger,
        PersistentComponentState? state = null)
    {
        _weatherService = weatherService;
        _logger = logger;
        _state = state;
    }

    /// <summary>
    /// Called asynchronously when the ViewModel is initialized. Restores or fetches weather forecasts and registers state persistence.
    /// </summary>
    public override async Task OnInitializedAsync()
    {
        if (_state is not null)
        {
            _persistingSubscription = _state.RegisterOnPersisting(PersistWeatherForecasts);

            if (_state.TryTakeFromJson<IEnumerable<WeatherForecast>>(
                nameof(WeatherForecasts), out var restored))
            {
                WeatherForecasts = restored;
                return;
            }
        }

        WeatherForecasts = await _weatherService.GetForecastAsync(_cancellationTokenSource.Token) ?? [];
    }

    /// <summary>
    /// Persists the current weather forecasts to the component state.
    /// </summary>
    /// <returns>A completed task.</returns>
    private Task PersistWeatherForecasts()
    {
        if (_state is not null)
        {
            _state.PersistAsJson(nameof(WeatherForecasts), WeatherForecasts);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes the ViewModel, releasing resources and detaching event handlers.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logger.LogInformation("Disposing {VMName}.", GetType().Name);
            _persistingSubscription.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        base.Dispose(disposing);
    }
}


