using Blazing.Common;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// Base ViewModel for navigation tests, providing navigation and query string processing logic.
/// </summary>
public abstract partial class TestNavigationBaseViewModel : ViewModelBase, ITestNavigationViewModel
{
    internal readonly IMvvmNavigationManager MvvmNavigationManager;
    internal readonly NavigationManager NavigationManager;

    private RelayCommand? _hexTranslateNavigateCommand;
    internal RelayCommand<string>? TestNavigateCommandImpl;

    /// <inheritdoc />
    [ObservableProperty]
    private string? _queryString;

    /// <inheritdoc />
    [ObservableProperty]
    private string? _test;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestNavigationBaseViewModel"/> class.
    /// </summary>
    /// <param name="mvvmNavigationManager">The MVVM navigation manager for view model-based navigation.</param>
    /// <param name="navigationManager">The Blazor navigation manager for URL-based navigation.</param>
    protected TestNavigationBaseViewModel(IMvvmNavigationManager mvvmNavigationManager, NavigationManager navigationManager)
    {
        MvvmNavigationManager = mvvmNavigationManager;
        NavigationManager = navigationManager;
        NavigationManager.LocationChanged += OnLocationChanged;
    }

    /// <inheritdoc />
    [ViewParameter] // populated by MvvmComponentBase
    public string? Echo { get; set; } = string.Empty;

    /// <inheritdoc />
    public RelayCommand HexTranslateNavigateCommand
        => _hexTranslateNavigateCommand ??= new RelayCommand(() => Navigate<HexTranslateViewModel>());

    /// <inheritdoc />
    public virtual RelayCommand<string> TestNavigateCommand
        => TestNavigateCommandImpl ??= new RelayCommand<string>(Navigate<ITestNavigationViewModel>);

    /// <inheritdoc />
    public override void OnInitialized()
        => ProcessQueryString();

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose(disposing);
    }

    /// <summary>
    /// Handles the <see cref="NavigationManager.LocationChanged"/> event and processes the query string.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        => ProcessQueryString();

    /// <summary>
    /// Navigates to the specified view model type with optional parameters.
    /// </summary>
    /// <typeparam name="T">The type of the view model to navigate to.</typeparam>
    /// <param name="@params">Optional navigation parameters.</param>
    private void Navigate<T>(string? @params = null)
        where T : IViewModelBase
    {
        if (string.IsNullOrWhiteSpace(@params))
        {
            MvvmNavigationManager.NavigateTo<T>();
            return;
        }

        MvvmNavigationManager.NavigateTo<T>(@params);
    }

    /// <summary>
    /// Processes the current query string and updates the <see cref="QueryString"/> and <see cref="Test"/> properties.
    /// </summary>
    private void ProcessQueryString()
    {
        QueryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
        NavigationManager.TryGetQueryString("test", out string temp);
        Test = temp;
    }
}
