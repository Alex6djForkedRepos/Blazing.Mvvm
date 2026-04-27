using Blazing.Common;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Individual item in a BootstrapRowGroup with automatic column sizing.
/// Columns parameter specifies actual Bootstrap columns (1-12).
/// </summary>
public partial class BootstrapRowGroupItem : ComponentControlBase
{
    /// <summary>
    /// The parent BootstrapRowGroup component.
    /// </summary>
    [CascadingParameter]
    public BootstrapRowGroup? Parent { get; set; }

    /// <summary>
    /// Explicit Bootstrap column width (1-12).
    /// If null, remaining space will be distributed evenly among null items.
    /// Values are actual Bootstrap columns, not weights.
    /// </summary>
    [Parameter]
    public int? Columns { get; set; }

    /// <summary>
    /// Optional override for column classes. If specified, overrides calculated columns.
    /// </summary>
    [Parameter]
    public string? ColumnClasses { get; set; }

    /// <summary>
    /// Initializes the item and registers with the parent group.
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Parent?.RegisterItem(this);
    }

    /// <summary>
    /// Re-renders the component after the parent has calculated column distribution.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            // Trigger re-render after parent calculates
            StateHasChanged();
        }
    }

    /// <summary>
    /// Builds the CSS classes for the column based on calculated or explicit size.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetColumnClasses()
    {
        // If explicit ColumnClasses provided, use those
        if (!string.IsNullOrEmpty(ColumnClasses))
        {
            return CssBuilder
                .Default(Class ?? ColumnClasses)
                .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
                .Build();
        }

        // Otherwise, get calculated columns from parent
        var calculatedColumns = Parent?.GetItemColumns(this) ?? 4;
        var baseClasses = GetResponsiveColumnClasses(calculatedColumns);

        return CssBuilder
            .Default(Class ?? baseClasses)
            .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
            .Build();
    }

    /// <summary>
    /// Generates responsive Bootstrap column classes based on column count.
    /// Mobile-first: full width on small screens, calculated width on medium+.
    /// </summary>
    /// <param name="columns">Number of Bootstrap columns (1-12)</param>
    /// <returns>Bootstrap responsive column classes</returns>
    private static string GetResponsiveColumnClasses(int columns)
    {
        return columns switch
        {
            1 => "col-12 col-md-1",
            2 => "col-12 col-md-2",
            3 => "col-12 col-md-3",
            4 => "col-12 col-md-4",
            5 => "col-12 col-md-5",
            6 => "col-12 col-md-6",
            7 => "col-12 col-md-7",
            8 => "col-12 col-md-8",
            9 => "col-12 col-md-9",
            10 => "col-12 col-md-10",
            11 => "col-12 col-md-11",
            12 => "col-12",
            _ => "col-12 col-md-4" // Default fallback
        };
    }
}



