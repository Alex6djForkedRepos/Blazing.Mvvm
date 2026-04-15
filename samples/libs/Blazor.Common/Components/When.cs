using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Blazing.Common.Components;

public class When : ComponentBase
{
    /// <summary>
    /// Condition function to evaluate the condition.
    /// </summary>
    [Parameter]
    public Func<bool> Condition { get; set; } = () => false;

    /// <summary>
    /// Content to render when predicate returns true.
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
        Parent?.AddWhen(this);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Do not render anything directly
    }
}
