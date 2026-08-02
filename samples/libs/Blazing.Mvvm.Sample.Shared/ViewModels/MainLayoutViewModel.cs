using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for the main layout, tracking navigation changes and a counter.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class MainLayoutViewModel : ViewModelBase
{
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Gets or sets the navigation change counter.
    /// </summary>
    [ObservableProperty]
    private int _counter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainLayoutViewModel"/> class.
    /// </summary>
    /// <param name="navigationManager">The navigation manager used to track location changes.</param>
    public MainLayoutViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    /// <summary>
    /// Handles the <see cref="NavigationManager.LocationChanged"/> event and increments the counter.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        => Counter++;

    /// <summary>
    /// Disposes the ViewModel and detaches event handlers.
    /// </summary>
    /// <param name="disposing">Indicates whether the method is called from Dispose.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose(disposing);
    }
}
