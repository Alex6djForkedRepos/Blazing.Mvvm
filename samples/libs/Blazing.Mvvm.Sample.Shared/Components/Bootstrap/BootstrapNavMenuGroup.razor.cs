using Blazing.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Collapsible navigation menu group component for use within BootstrapNavMenu.
/// Automatically expands when any child link is active.
/// </summary>
public partial class BootstrapNavMenuGroup : ComponentControlBase, IAsyncDisposable
{
    private ElementReference _contentRef;
    private IJSObjectReference? _jsModule;
    private bool _hasCheckedForActiveLink;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    [CascadingParameter]
    private BootstrapNavMenu? ParentMenu { get; set; }

    /// <summary>
    /// The title of the navigation group (will be displayed in uppercase).
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for the collapse element. If not provided, one will be generated.
    /// </summary>
    [Parameter]
    public string? CollapseId { get; set; }

    /// <summary>
    /// Gets whether the group is currently expanded.
    /// </summary>
    private bool IsExpanded => ParentMenu?.IsGroupExpanded(Title) ?? false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        CollapseId ??= $"nav-group-{Title.Replace(" ", "-").ToLowerInvariant()}";
        
        // Register this group with the parent
        ParentMenu?.RegisterGroup(Title, this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        
        if (firstRender)
        {
            try
            {
                // Load the JavaScript module using RCL path format (only once)
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/Blazing.Mvvm.Sample.Shared/Components/Bootstrap/BootstrapNavMenuGroup.razor.js");
            }
            catch (Exception ex)
            {
                // Log or handle JS interop errors
                Console.WriteLine($"Error loading JS module: {ex.Message}");
            }
        }
        
        // Check on every render if we haven't checked yet OR if the module is loaded
        if (_jsModule is not null && !_hasCheckedForActiveLink)
        {
            _hasCheckedForActiveLink = true;
            await CheckAndNotifyParent();
        }
    }
    
    /// <summary>
    /// Resets the check flag so the group can re-check for active links on next render.
    /// Called by parent on navigation.
    /// </summary>
    internal void ResetActiveCheck()
    {
        _hasCheckedForActiveLink = false;
    }

    /// <summary>
    /// Checks if this group contains any links with the 'active' class and notifies parent.
    /// </summary>
    internal async Task CheckAndNotifyParent()
    {
        if (_jsModule is null)
            return;

        try
        {
            var hasActive = await _jsModule.InvokeAsync<bool>("hasActiveLink", _contentRef);
            if (hasActive)
            {
                // Notify parent to expand this group
                ParentMenu?.ExpandGroup(Title);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking active link: {ex.Message}");
        }
    }

    private void Toggle()
    {
        ParentMenu?.ToggleGroup(Title);
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_jsModule is not null)
        {
            try
            {
                await _jsModule.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
        
        Dispose();
    }
}
