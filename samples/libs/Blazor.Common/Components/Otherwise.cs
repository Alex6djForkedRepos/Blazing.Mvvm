using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazing.Common.Components;

public class Otherwise : ComponentBase
{
    /// <summary>
    /// Content to render when no When conditions are true.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Parent ConditionalSwitch component.
    /// </summary>
    [CascadingParameter]
    public ConditionalSwitch? Parent { get; set; }

    protected override void OnInitialized()
    {
        Parent?.SetOtherwise(this);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Do not render anything directly
    }
}
