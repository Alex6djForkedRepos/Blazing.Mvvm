using Microsoft.AspNetCore.Components;

namespace Blazing.Common.Components;

public partial class ConditionalSwitch : ComponentBase
{
    private RenderFragment? _renderFragment;
    private readonly List<When> _whenCases = new();
    private Otherwise? _otherwise;

    /// <summary>
    /// Child content containing When and Otherwise components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    internal void AddWhen(When when)
    {
        _whenCases.Add(when);
        UpdateRenderFragment();
        InvokeAsync(StateHasChanged);
    }

    internal void SetOtherwise(Otherwise otherwise)
    {
        _otherwise = otherwise;
        UpdateRenderFragment();
        InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        UpdateRenderFragment();
    }

    private void UpdateRenderFragment()
    {
        // Find first matching When case by invoking predicates
        var matchingCase = _whenCases.FirstOrDefault(w => w.Condition());
        if (matchingCase != null)
        {
            _renderFragment = matchingCase.ChildContent;
        }
        else if (_otherwise != null)
        {
            _renderFragment = _otherwise.ChildContent;
        }
        else
        {
            _renderFragment = null;
        }
    }
}
