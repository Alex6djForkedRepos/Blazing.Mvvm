using Blazing.Common;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Bootstrap accordion item component.
/// </summary>
public partial class BootstrapAccordionItem : ComponentControlBase
{
    private const string _DEFAULT_BASE_CLASSES = "accordion-item";

    /// <summary>
    /// The header content or title for the accordion item.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment Header { get; set; } = null!;

    /// <summary>
    /// Optional icon class to display before the header (e.g., "bi bi-lightning").
    /// </summary>
    [Parameter]
    public string? HeaderIcon { get; set; }

    /// <summary>
    /// The body content for the accordion item.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment Body { get; set; } = null!;

    /// <summary>
    /// The ID of the parent accordion. Required for proper accordion behavior.
    /// </summary>
    [Parameter, EditorRequired]
    public string ParentId { get; set; } = null!;

    /// <summary>
    /// Unique identifier for the collapse element. If not provided, one will be generated.
    /// </summary>
    [Parameter]
    public string? CollapseId { get; set; }

    /// <summary>
    /// Whether the accordion item should be expanded by default.
    /// </summary>
    [Parameter]
    public bool IsExpanded { get; set; }

    /// <summary>
    /// Initializes the accordion item and generates a unique collapse ID if not provided.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        CollapseId ??= $"collapse-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Builds the CSS classes for the accordion item container.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetClasses()
        => CssBuilder
            .Default(Class ?? _DEFAULT_BASE_CLASSES)
            .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
            .Build();

    /// <summary>
    /// Builds the CSS classes for the accordion button.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetButtonClasses()
        => CssBuilder
            .Default("accordion-button")
            .AddClass("collapsed", () => !IsExpanded)
            .Build();

    /// <summary>
    /// Builds the CSS classes for the collapse container.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetCollapseClasses()
        => CssBuilder
            .Default("accordion-collapse collapse")
            .AddClass("show", () => IsExpanded)
            .Build();
}
