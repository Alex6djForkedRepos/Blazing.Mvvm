using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazing.Mvvm.Tests.Components.ViewModels;

public sealed partial class MainLayoutViewModel : ViewModelBase
{
    private readonly NavigationManager _navigationManager;

    [ObservableProperty]
    private int _counter;

    public MainLayoutViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose(disposing);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        => Counter++;
}
