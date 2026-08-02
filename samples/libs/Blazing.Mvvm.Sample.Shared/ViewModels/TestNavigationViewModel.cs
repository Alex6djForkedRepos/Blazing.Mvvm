using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for demonstrating navigation using MVVM navigation manager and Blazor navigation manager.
/// </summary>
[ViewModelDefinition<ITestNavigationViewModel>(Lifetime = ServiceLifetime.Scoped)]
public sealed class TestNavigationViewModel(IMvvmNavigationManager mvvmNavigationManager, NavigationManager navigationManager)
    : TestNavigationBaseViewModel(mvvmNavigationManager, navigationManager)
{ /* skipped */ }