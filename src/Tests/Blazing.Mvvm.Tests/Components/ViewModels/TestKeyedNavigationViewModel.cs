using System.Web;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazing.Mvvm.Tests.Components.ViewModels;

[ViewModelDefinition<ITestKeyedNavigationViewModel>(Key = nameof(TestKeyedNavigationViewModel))]
public partial class TestKeyedNavigationViewModel : ViewModelBase, ITestKeyedNavigationViewModel
{
    private readonly IMvvmNavigationManager _mvvmNavigationManager;
    private readonly NavigationManager _navigationManager;

    private RelayCommand? _hexTranslateNavigateCommand;
    private RelayCommand<string>? _testNavigateCommand;

    [ObservableProperty]
    private string? _queryString;

    [ObservableProperty]
    private string? _test;

    public TestKeyedNavigationViewModel(IMvvmNavigationManager mvvmNavigationManager, NavigationManager navigationManager)
    {
        _mvvmNavigationManager = mvvmNavigationManager;
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    [ViewParameter]
    public string? Echo { get; set; } = string.Empty;

    public RelayCommand HexTranslateNavigateCommand
        => _hexTranslateNavigateCommand ??= new RelayCommand(() => Navigate<HexTranslateViewModel>());

    public RelayCommand<string> TestNavigateCommand
        => _testNavigateCommand ??= new RelayCommand<string>(s => Navigate(nameof(TestKeyedNavigationViewModel), s));

    public override void OnInitialized()
        => ProcessQueryString();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }

        base.Dispose(disposing);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        => ProcessQueryString();

    private void Navigate<T>(string? @params = null)
        where T : IViewModelBase
    {
        if (string.IsNullOrWhiteSpace(@params))
        {
            _mvvmNavigationManager.NavigateTo<T>();
            return;
        }

        _mvvmNavigationManager.NavigateTo<T>(@params);
    }

    private void Navigate(string key, string? @params = null)
    {
        if (string.IsNullOrWhiteSpace(@params))
        {
            _mvvmNavigationManager.NavigateTo(key);
            return;
        }

        _mvvmNavigationManager.NavigateTo(key, @params);
    }

    private void ProcessQueryString()
    {
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        QueryString = uri.Query;

        if (!string.IsNullOrEmpty(uri.Query))
        {
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            Test = queryParams["test"];
        }
        else
        {
            Test = null;
        }
    }
}
