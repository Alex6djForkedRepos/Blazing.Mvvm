using Blazing.Common;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Bootstrap accordion container component.
/// </summary>
public partial class BootstrapAccordion : ComponentControlBase
{
    private const string _DEFAULT_BASE_CLASSES = "accordion";

    /// <summary>
    /// The unique identifier for the accordion. Used to group accordion items.
    /// If not provided, a unique ID will be generated.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Initializes the accordion component and generates a unique ID if not provided.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Id ??= $"accordion-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Builds the CSS classes for the accordion.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetClasses()
        => CssBuilder
            .Default(Class ?? _DEFAULT_BASE_CLASSES)
            .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
            .Build();
}
