using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Blazing.Mvvm.Sample.Shared.ViewModels.ParameterResolution;
using Blazing.Mvvm.Sample.Shared.ViewModels.ParentChild;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for the sample application info, providing navigation logic to various pages and view models.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class SampleAppInfoViewModel : ViewModelBase
{
    private readonly IMvvmNavigationManager _mvvmNavigationManager;
    private readonly NavigationManager _navigationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleAppInfoViewModel"/> class.
    /// </summary>
    /// <param name="mvvmNavigationManager">The MVVM navigation manager for view model-based navigation.</param>
    /// <param name="navigationManager">The Blazor navigation manager for URL-based navigation.</param>
    public SampleAppInfoViewModel(IMvvmNavigationManager mvvmNavigationManager, NavigationManager navigationManager)
    {
        _mvvmNavigationManager = mvvmNavigationManager;
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Navigates to the specified page or view model based on the provided page key.
    /// </summary>
    /// <param name="page">The page key or identifier to navigate to.</param>
    [RelayCommand]
    private void Navigation(string? page)
    {
        if (string.IsNullOrWhiteSpace(page))
            return;

        switch (page.ToLowerInvariant())
        {
            case "relaycommands":
                // do not use "/relaycommands" as it bypasses PathBase via <base href> for the routing system - very important for SubPaths!
                _navigationManager.NavigateTo("relaycommands");
                break;
            case "users":
                _mvvmNavigationManager.NavigateTo<UsersViewModel>();
                break;
            case "test":
                _mvvmNavigationManager.NavigateTo<ITestNavigationViewModel>();
                break;
            case "keyedtest":
                _mvvmNavigationManager.NavigateTo(nameof(TestKeyedNavigationViewModel));
                break;
            case "hextranslate":
                _mvvmNavigationManager.NavigateTo<HexTranslateViewModel>();
                break;
            case "form":
                _mvvmNavigationManager.NavigateTo<EditContactViewModel>();
                break;
            case "fetchdata":
                _mvvmNavigationManager.NavigateTo<FetchDataViewModel>();
                break;
            case "paramresoverview": 
                _mvvmNavigationManager.NavigateTo<ParameterResolutionOverviewViewModel>();
                break;
            case "parameterresolution":
                _mvvmNavigationManager.NavigateTo<ParameterDemoViewModel>();
                break;
            case "parentchild":
                _mvvmNavigationManager.NavigateTo<ParentChildViewModel>();
                break;
        }
    }
}
