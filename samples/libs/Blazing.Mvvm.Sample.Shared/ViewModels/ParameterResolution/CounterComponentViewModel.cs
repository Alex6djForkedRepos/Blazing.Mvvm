using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blazing.Mvvm.Sample.Shared.ViewModels.ParameterResolution;

/// <summary>
/// ViewModel for a counter component, providing increment and decrement commands and a counter value.
/// </summary>
public partial class CounterComponentViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the current counter value. This value is passed as a view parameter.
    /// </summary>
    [ObservableProperty]
    [property: ViewParameter]
    private int _counter;

    /// <summary>
    /// Increments the counter value by one.
    /// </summary>
    [RelayCommand]
    private void IncrementCounter()
    {
        Counter++;
    }

    /// <summary>
    /// Decrements the counter value by one.
    /// </summary>
    [RelayCommand]
    private void DecrementCounter()
    {
        Counter--;
    }
}
