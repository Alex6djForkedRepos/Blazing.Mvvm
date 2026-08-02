using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blazing.Mvvm.Tests.Components.ViewModels;

public interface ITestKeyedNavigationViewModel : IViewModelBase, IDisposable
{
    RelayCommand<string> TestNavigateCommand { get; }

    string? QueryString { get; set; }

    string? Test { get; set; }

    string? Echo { get; set; }

    RelayCommand HexTranslateNavigateCommand { get; }
}
