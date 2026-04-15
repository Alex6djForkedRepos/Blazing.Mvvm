using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// Interface for a ViewModel supporting navigation and command-based navigation logic.
/// </summary>
public interface ITestNavigationViewModel : IViewModelBase, IDisposable
{
    /// <summary>
    /// Gets or sets the query string used for navigation.
    /// </summary>
    string QueryString { get; set; }

    /// <summary>
    /// Gets or sets the test value used for navigation.
    /// </summary>
    string Test { get; set; }

    /// <summary>
    /// Gets or sets the echo value, populated by the component base.
    /// </summary>
    string? Echo { get; set; }

    /// <summary>
    /// Gets the command to navigate to the hex translate view.
    /// </summary>
    RelayCommand HexTranslateNavigateCommand { get; }

    /// <summary>
    /// Gets the command to navigate using a test key.
    /// </summary>
    RelayCommand<string> TestNavigateCommand { get; }
}
