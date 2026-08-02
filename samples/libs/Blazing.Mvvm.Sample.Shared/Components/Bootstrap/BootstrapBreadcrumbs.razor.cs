using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazing.Mvvm.Sample.Shared.Components.Bootstrap;

/// <summary>
/// Bootstrap 5.3 breadcrumb component that automatically generates breadcrumbs from the current URL.
/// Supports custom labels for specific segments via the CustomLabels parameter.
/// </summary>
public partial class BootstrapBreadcrumbs : ComponentBase, IDisposable
{
    [Inject] private NavigationManager Nav { get; set; } = null!;

    /// <summary>
    /// Additional CSS classes for the nav container.
    /// </summary>
    [Parameter] public string? AdditionalClasses { get; set; }

    /// <summary>
    /// Additional CSS classes for the breadcrumb ol element.
    /// </summary>
    [Parameter] public string? BreadcrumbClasses { get; set; }
    
    /// <summary>
    /// Custom labels for specific URL segments. Key can be the segment value (e.g., "1", "202") 
    /// or the segment name (e.g., "users", "posts"). Value is the display text.
    /// Example: new() { ["1"] = "Alice Johnson", ["202"] = "My First Post" }
    /// </summary>
    [Parameter] public Dictionary<string, string>? CustomLabels { get; set; }

    private readonly List<(string Text, string Url)> _breadcrumbs = [];

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
        BuildBreadcrumbs();
    }

    protected override void OnParametersSet()
    {
        BuildBreadcrumbs();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        BuildBreadcrumbs();
        StateHasChanged();
    }

    private void BuildBreadcrumbs()
    {
        _breadcrumbs.Clear();
        
        // Use ToBaseRelativePath to automatically handle PathBase
        var path = Nav.ToBaseRelativePath(Nav.Uri).TrimStart('/');
        
        // Don't show breadcrumbs on home page
        if (string.IsNullOrEmpty(path))
            return;

        // Always start with Home (empty string respects PathBase via <base href>)
        _breadcrumbs.Add(("Home", ""));

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "";

        for (int i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];
            currentPath += $"/{segment}";

            // Use CustomLabels if provided, otherwise format the segment
            string label;
            if (CustomLabels?.TryGetValue(segment, out var customLabel) == true ||
                CustomLabels?.TryGetValue(segment.ToLower(), out customLabel) == true)
            {
                label = customLabel;
            }
            else
            {
                label = FormatSegment(segment, i > 0 ? segments[i - 1] : null);
            }

            _breadcrumbs.Add((label, currentPath));
        }
    }

    private static string FormatSegment(string segment, string? previousSegment)
    {
        // If it's a number and previous segment exists, it's likely an ID
        if (int.TryParse(segment, out var id) && !string.IsNullOrEmpty(previousSegment))
        {
            var entityName = previousSegment.TrimEnd('s'); // Simple singularization
            return $"{Capitalize(entityName)} {id}";
        }

        return Capitalize(segment);
    }

    private static string Capitalize(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
        
        return char.ToUpper(text[0]) + text[1..];
    }

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
    }
}
