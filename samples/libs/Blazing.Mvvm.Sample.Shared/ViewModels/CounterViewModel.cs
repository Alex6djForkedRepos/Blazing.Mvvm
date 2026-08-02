using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazing.Mvvm.Sample.Shared.ViewModels;

/// <summary>
/// ViewModel for a counter component, providing increment, reset, and life-cycle logging functionality.
/// </summary>
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class CounterViewModel : ViewModelBase
{
    private readonly ILogger<CounterViewModel> _logger;

    /// <summary>
    /// Gets or sets the current count value.
    /// </summary>
    [ObservableProperty]
    private int _currentCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="CounterViewModel"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging life-cycle events.</param>
    public CounterViewModel(ILogger<CounterViewModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Increments the current count by one.
    /// </summary>
    public void IncrementCount()
        => CurrentCount++;

    /// <summary>
    /// Resets the current count to zero.
    /// </summary>
    public void ResetCount()
        => CurrentCount = 0;

    /// <summary>
    /// Called when the ViewModel is initialized.
    /// </summary>
    public override void OnInitialized()
        => LogLifeCycleEvent(nameof(OnInitialized));

    /// <summary>
    /// Called asynchronously when the ViewModel is initialized.
    /// </summary>
    public override Task OnInitializedAsync()
    {
        LogLifeCycleEvent(nameof(OnInitializedAsync));
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Called when component parameters are set.
    /// </summary>
    public override void OnParametersSet()
        => LogLifeCycleEvent(nameof(OnParametersSet));

    /// <summary>
    /// Called asynchronously when component parameters are set.
    /// </summary>
    public override Task OnParametersSetAsync()
    {
        LogLifeCycleEvent(nameof(OnParametersSetAsync));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after the component has rendered.
    /// </summary>
    /// <param name="firstRender">Indicates if this is the first render.</param>
    public override void OnAfterRender(bool firstRender)
        => LogLifeCycleEvent(nameof(OnAfterRender));

    /// <summary>
    /// Called asynchronously after the component has rendered.
    /// </summary>
    /// <param name="firstRender">Indicates if this is the first render.</param>
    public override Task OnAfterRenderAsync(bool firstRender)
    {
        LogLifeCycleEvent(nameof(OnAfterRenderAsync));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines whether the component should render.
    /// </summary>
    /// <returns><c>true</c> if the component should render; otherwise, <c>false</c>.</returns>
    public override bool ShouldRender()
    {
        LogLifeCycleEvent(nameof(ShouldRender));
        return true;
    }

    /// <summary>
    /// Logs a life-cycle event for the ViewModel.
    /// </summary>
    /// <param name="lifeCycleMethod">The name of the life-cycle method.</param>
    /// <param name="viewModel">The name of the ViewModel.</param>
    /// <param name="level">The log level.</param>
    [LoggerMessage(Message = "{ViewModel} => Life-cycle event: {LifeCycleMethod}.")]
    private partial void LogLifeCycleEvent(string lifeCycleMethod, string viewModel = nameof(CounterViewModel), LogLevel level = LogLevel.Information);
}
