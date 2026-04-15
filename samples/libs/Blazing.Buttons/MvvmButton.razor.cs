using Blazing.Common;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;

namespace Blazing.Buttons;

public partial class MvvmButton : ComponentControlBase
{
    private const string DEFAULT_BUTTON_TYPE = "button";
    private const string DEFAULT_BASE_CLASSES = "mvvm-button";

    /// <summary>
    /// Icon markup to display (supports any icon system, e.g. Material Symbols))
    /// </summary>
    [Parameter]
    public RenderFragment? Icon { get; set; }

    /// <summary>
    /// Button text label
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Tooltip title
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// HTML button type (button, submit, reset)
    /// </summary>
    [Parameter]
    public string ButtonType { get; set; } = DEFAULT_BUTTON_TYPE;

    /// <summary>
    /// RelayCommand to execute (sync or async)
    /// </summary>
    [Parameter]
    public IRelayCommand? Command { get; set; }

    /// <summary>
    /// Parameter to pass to the RelayCommand
    /// </summary>
    [Parameter]
    public object? CommandParameter { get; set; }

    /// <summary>
    /// Manual disabled state (overrides RelayCommand CanExecute)
    /// </summary>
    [Parameter]
    public bool? Disabled { get; set; }

    /// <summary>
    /// Additional click handler (invoked after command execution)
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// Whether button is disabled (either manually or via RelayCommand CanExecute)
    /// </summary>
    private bool IsDisabled => Disabled == true || (Command != null && !Command.CanExecute(CommandParameter));

    /// <summary>
    /// Handles button click events, executing the command if present and invoking additional click handler
    /// </summary>
    private async Task HandleClick()
    {
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
            // Wait for async commands to complete
            if (Command is IAsyncRelayCommand { IsRunning: true } asyncCommand)
            {
                await asyncCommand.ExecutionTask!;
            }
        }
        // Invoke additional click handler
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }

    /// <summary>
    /// Builds the CSS classes for the button 
    /// </summary>
    /// <returns></returns>
    private string GetClasses()
        => CssBuilder
            .Default(Class ?? DEFAULT_BASE_CLASSES)
            .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
            .Build();
}
