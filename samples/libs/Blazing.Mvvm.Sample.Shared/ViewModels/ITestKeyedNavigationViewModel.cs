using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// Interface for a ViewModel supporting keyed navigation and command-based navigation logic.
/// </summary>
public interface ITestKeyedNavigationViewModel : IViewModelBase
{
    /// <summary>
    /// Gets the command to navigate using a test key.
    /// </summary>
    RelayCommand<string> TestNavigateCommand { get; }

    /// <summary>
    /// Gets or sets the query string used for navigation.
    /// </summary>
    /// <remarks>See <see cref="TestNavigationBaseViewModel._queryString"/> for backing field.</remarks>
    string? QueryString { get; set; }

    /// <summary>
    /// Gets or sets the test value used for navigation.
    /// </summary>
    /// <remarks>See <see cref="TestNavigationBaseViewModel._test"/> for backing field.</remarks>
    string? Test { get; set; }

    /// <summary>
    /// Gets or sets the echo value, populated by the component base.
    /// </summary>
    string? Echo { get; set; }

    /// <summary>
    /// Gets the command to navigate to the hex translate view.
    /// </summary>
    RelayCommand HexTranslateNavigateCommand { get; }
}
