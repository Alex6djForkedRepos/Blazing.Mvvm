using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for demonstrating keyed navigation using MVVM navigation manager.
/// </summary>
[ViewModelDefinition<ITestKeyedNavigationViewModel>(Key = nameof(TestKeyedNavigationViewModel))]
public sealed class TestKeyedNavigationViewModel(IMvvmNavigationManager mvvmNavigationManager, NavigationManager navigationManager)
    : TestNavigationBaseViewModel(mvvmNavigationManager, navigationManager), ITestKeyedNavigationViewModel
{
    /// <inheritdoc />
    public override RelayCommand<string> TestNavigateCommand
        => TestNavigateCommandImpl ??= new RelayCommand<string>(s => Navigate(nameof(TestKeyedNavigationViewModel), s));

    /// <summary>
    /// Navigates to the specified key and optional parameters using the MVVM navigation manager.
    /// </summary>
    /// <param name="key">The navigation key.</param>
    /// <param name="@params">Optional navigation parameters.</param>
    private void Navigate(string key, string? @params = null)
    {
        if (string.IsNullOrWhiteSpace(@params))
        {
            MvvmNavigationManager.NavigateTo(key);
            return;
        }

        MvvmNavigationManager.NavigateTo(key, @params);
    }
}
