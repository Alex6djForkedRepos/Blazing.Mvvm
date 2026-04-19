using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Tests.Components.Models;
using Blazing.Mvvm.Tests.Components.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Tests.Components.ViewModels;

[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class FetchDataViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<FetchDataViewModel> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    [ObservableProperty]
    private IEnumerable<WeatherForecast>? _weatherForecasts;

    public FetchDataViewModel(IWeatherService weatherService, ILogger<FetchDataViewModel> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    public Task PersistStateAsync(PersistentComponentState state)
    {
        state.PersistAsJson(nameof(WeatherForecasts), WeatherForecasts);
        return Task.CompletedTask;
    }

    public async Task LoadStateAsync(PersistentComponentState state)
    {
        if (state.TryTakeFromJson<IEnumerable<WeatherForecast>>(nameof(WeatherForecasts), out var weatherForecasts))
        {
            WeatherForecasts = weatherForecasts!;
        }
        else
        {
            WeatherForecasts = await _weatherService.GetForecastAsync(_cancellationTokenSource.Token) ?? [];
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logger.LogInformation("Disposing {VMName}.", nameof(FetchDataViewModel));
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        base.Dispose(disposing);
    }
}
