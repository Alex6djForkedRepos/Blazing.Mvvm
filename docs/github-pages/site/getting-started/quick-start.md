# Getting Started

This guide gets Blazing.Mvvm running in a Blazor app, then shows the minimum ViewModel and component setup needed to render data through MVVM.

## 1. Install the package

Add the [Blazing.Mvvm](https://www.nuget.org/packages/Blazing.Mvvm) package to your project.

### .NET CLI

```bash
dotnet add package Blazing.Mvvm
```

### NuGet Package Manager

```powershell
Install-Package Blazing.Mvvm
```

## 2. Register Blazing.Mvvm

Configure the library in `Program.cs` with [`AddMvvm`](xref:Blazing.Mvvm.ServicesExtension.AddMvvm*):

```csharp
using Blazing.Mvvm;

builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.WebApp;
});
```

Choose the hosting model that matches your application:

- [`BlazorHostingModelType.Server`](xref:Blazing.Mvvm.BlazorHostingModelType.Server)
- [`BlazorHostingModelType.WebAssembly`](xref:Blazing.Mvvm.BlazorHostingModelType.WebAssembly)
- [`BlazorHostingModelType.WebApp`](xref:Blazing.Mvvm.BlazorHostingModelType.WebApp)
- [`BlazorHostingModelType.Hybrid`](xref:Blazing.Mvvm.BlazorHostingModelType.Hybrid)
- [`BlazorHostingModelType.HybridMaui`](xref:Blazing.Mvvm.BlazorHostingModelType.HybridMaui)

> [!NOTE]
> Since v3.1.0, `BasePath` is automatically detected from the application's base URI in common subpath and YARP scenarios. See [Subpath Hosting](../hosting/subpath-hosting.md) for details.

## 3. Register ViewModels from another assembly when needed

If your ViewModels live outside the calling assembly, register the assembly explicitly:

```csharp
using Blazing.Mvvm;

builder.Services.AddMvvm(options =>
{
    options.RegisterViewModelsFromAssemblyContaining<MyViewModel>();
});

// OR

var vmAssembly = typeof(MyViewModel).Assembly;
builder.Services.AddMvvm(options =>
{
    options.RegisterViewModelsFromAssembly(vmAssembly);
});
```

For more advanced multi-assembly patterns, see [Multi-Project ViewModel Registration](../configuration/multi-project-registration.md).

## 4. Create a ViewModel

Create a ViewModel that inherits from [`ViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ViewModelBase):

```csharp
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class FetchDataViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<FetchDataViewModel> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    [ObservableProperty]
    private IEnumerable<WeatherForecast>? _weatherForecasts;

    public string Title => "Weather forecast";

    public FetchDataViewModel(IWeatherService weatherService, ILogger<FetchDataViewModel> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    public override async Task OnInitializedAsync()
    {
        WeatherForecasts = await _weatherService.GetForecastAsync() ?? [];
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
```

See [View Models](../configuration/view-models.md) for base class selection, lifecycle methods, disposal, and registration options.

## 5. Create a component that uses the ViewModel

Use [`MvvmComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmComponentBase`1) or [`MvvmOwningComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmOwningComponentBase`1) in your Razor component:

```razor
@page "/fetchdata"
@inherits MvvmOwningComponentBase<FetchDataViewModel>

<PageTitle>@ViewModel.Title</PageTitle>

<h1>@ViewModel.Title</h1>

@if (ViewModel.WeatherForecasts is null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in ViewModel.WeatherForecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}
```

> [!NOTE]
> Use [`MvvmOwningComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmOwningComponentBase`1) when the component needs its own DI scope for repositories, DbContexts, or other scoped dependencies.

## What to read next

- [View Models](../configuration/view-models.md)
- [Parameter Resolution and Two-Way Binding](../configuration/parameter-resolution.md)
- [MVVM Navigation](../navigation/mvvm-navigation.md)
- [Sample Projects](../samples/sample-projects.md)
