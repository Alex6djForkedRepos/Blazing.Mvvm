using Blazing.Common;
using Microsoft.AspNetCore.Components;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Bootstrap row group component that distributes items across a 12-column grid.
/// Automatically scales column sizes to always fill the full 12-column Bootstrap row.
/// </summary>
public partial class BootstrapRowGroup : ComponentControlBase
{
    private const string _DEFAULT_BASE_CLASSES = "row";
    private const string DEFAULT_GAP_CLASS = "g-4";
    private const string DEFAULT_MARGIN_CLASS = "mb-4";
    private const int MAX_COLUMNS = 12;

    private readonly List<BootstrapRowGroupItem> _items = [];
    private Dictionary<BootstrapRowGroupItem, int> _calculatedColumns = [];
    private bool _isCalculated;
    private bool _isVisible;

    /// <summary>
    /// Gap spacing between items. Default is "g-4" (1.5rem gap).
    /// Common values: g-0, g-1, g-2, g-3, g-4, g-5
    /// </summary>
    [Parameter]
    public string GapClass { get; set; } = DEFAULT_GAP_CLASS;

    /// <summary>
    /// Bottom margin for the entire group. Default is "mb-4".
    /// </summary>
    [Parameter]
    public string MarginClass { get; set; } = DEFAULT_MARGIN_CLASS;

    /// <summary>
    /// Registers an item with the row group and triggers column calculation.
    /// </summary>
    internal void RegisterItem(BootstrapRowGroupItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
            _isCalculated = false;
        }
    }

    /// <summary>
    /// Called after the component has rendered. Calculates column distribution after all items have registered.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!_isCalculated && _items.Count > 0)
        {
            CalculateColumnDistribution();
            _isCalculated = true;
            _isVisible = true;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Gets the calculated column size for a specific item.
    /// </summary>
    internal int GetItemColumns(BootstrapRowGroupItem item)
    {
        return _calculatedColumns.TryGetValue(item, out var columns) ? columns : 1;
    }

    /// <summary>
    /// Distributes 12 Bootstrap columns among all items.
    /// Uses actual Bootstrap column values (1-12), scaling to fit when necessary.
    /// </summary>
    private void CalculateColumnDistribution()
    {
        if (_items.Count == 0)
        {
            _calculatedColumns = [];
            return;
        }

        var finalColumns = new Dictionary<BootstrapRowGroupItem, int>();
        var explicitItems = _items.Where(i => i.Columns.HasValue).ToList();
        var nullItems = _items.Where(i => !i.Columns.HasValue).ToList();

        // Case 1: All items have null columns ? distribute 12 evenly
        if (nullItems.Count == _items.Count)
        {
            var columnsPerItem = MAX_COLUMNS / _items.Count;
            var remainder = MAX_COLUMNS % _items.Count;

            for (int i = 0; i < _items.Count; i++)
            {
                finalColumns[_items[i]] = columnsPerItem + (i < remainder ? 1 : 0);
            }
        }
        // Case 2: Mixed (some explicit, some null)
        else if (nullItems.Count > 0)
        {
            var explicitSum = explicitItems.Sum(i => i.Columns!.Value);
            var remaining = MAX_COLUMNS - explicitSum;

            // Assign explicit columns
            foreach (var item in explicitItems)
            {
                finalColumns[item] = item.Columns!.Value;
            }

            // Distribute remaining among null items
            if (remaining > 0 && nullItems.Count > 0)
            {
                var columnsPerNull = remaining / nullItems.Count;
                var nullRemainder = remaining % nullItems.Count;

                for (int i = 0; i < nullItems.Count; i++)
                {
                    finalColumns[nullItems[i]] = columnsPerNull + (i < nullRemainder ? 1 : 0);
                }
            }
            else
            {
                // No remaining space or negative, scale everything down
                ScaleToFit(finalColumns);
            }
        }
        // Case 3: All items have explicit columns
        else
        {
            var explicitSum = explicitItems.Sum(i => i.Columns!.Value);

            if (explicitSum == MAX_COLUMNS)
            {
                // Perfect fit!
                foreach (var item in explicitItems)
                {
                    finalColumns[item] = item.Columns!.Value;
                }
            }
            else
            {
                // Scale to fit 12 columns
                foreach (var item in explicitItems)
                {
                    finalColumns[item] = item.Columns!.Value;
                }
                ScaleToFit(finalColumns);
            }
        }

        _calculatedColumns = finalColumns;
    }

    /// <summary>
    /// Scales all columns proportionally to fit exactly 12 columns.
    /// Ensures same-sized items stay the same size after scaling.
    /// </summary>
    private void ScaleToFit(Dictionary<BootstrapRowGroupItem, int> columns)
    {
        var currentTotal = columns.Values.Sum();
        if (currentTotal == MAX_COLUMNS) return;

        // Scale factor
        var scale = (double)MAX_COLUMNS / currentTotal;

        // Calculate scaled values
        var scaled = new Dictionary<BootstrapRowGroupItem, double>();
        foreach (var item in columns.Keys)
        {
            scaled[item] = columns[item] * scale;
        }

        // Round to integers while maintaining total = 12
        var rounded = new Dictionary<BootstrapRowGroupItem, int>();
        foreach (var item in columns.Keys)
        {
            rounded[item] = (int)Math.Floor(scaled[item]);
        }

        // Distribute remainder to largest items
        var diff = MAX_COLUMNS - rounded.Values.Sum();
        var sortedByRemainder = scaled
            .OrderByDescending(kv => scaled[kv.Key] - rounded[kv.Key])
            .ThenByDescending(kv => scaled[kv.Key])
            .Select(kv => kv.Key)
            .ToList();

        for (int i = 0; i < diff && i < sortedByRemainder.Count; i++)
        {
            rounded[sortedByRemainder[i]]++;
        }

        // Update columns with scaled values
        foreach (var item in columns.Keys.ToList())
        {
            columns[item] = Math.Max(1, rounded[item]); // Min 1 column
        }
    }

    /// <summary>
    /// Builds the CSS classes for the row container.
    /// </summary>
    /// <returns>The combined CSS classes.</returns>
    private string GetRowClasses()
        => CssBuilder
            .Default(Class ?? _DEFAULT_BASE_CLASSES)
            .AddClass(GapClass)
            .AddClass(MarginClass)
            .AddClass("invisible", () => !_isVisible)
            .AddClass(AdditionalClasses!, () => !string.IsNullOrEmpty(AdditionalClasses))
            .Build();
}


