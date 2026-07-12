# MVVM Navigation

Blazing.Mvvm adds ViewModel-aware navigation on top of Blazor's `NavigationManager`. Instead of hard-coding route strings across your app, you can navigate by ViewModel type, abstraction, or key.

## How navigation works

When [`MvvmNavigationManager`](xref:Blazing.Mvvm.Components.MvvmNavigationManager) is initialized as a singleton, it scans assemblies and caches the relationships between ViewModels and pages. Navigation then becomes a lookup from ViewModel metadata to a route.

> [!NOTE]
> [`MvvmNavigationManager`](xref:Blazing.Mvvm.Components.MvvmNavigationManager) extends MVVM scenarios. It does not replace every use of Blazor's built-in `NavigationManager`.

## Use `MvvmNavLink` in navigation UI

Replace route-based `NavLink` usage with [`MvvmNavLink`](xref:Blazing.Mvvm.Components.Routing.MvvmNavLink`1):

```razor
<div class="nav-item px-3">
    <MvvmNavLink class="nav-link" TViewModel="FetchDataViewModel">
        <span class="oi oi-list-rich" aria-hidden="true"></span> Fetch data
    </MvvmNavLink>
</div>
```

[`MvvmNavLink`](xref:Blazing.Mvvm.Components.Routing.MvvmNavLink`1) is based on Blazor's `NavLink`, but adds `TViewModel` and `RelativeUri`.

## Navigate from code

Inject [`MvvmNavigationManager`](xref:Blazing.Mvvm.Components.MvvmNavigationManager) and navigate by ViewModel type:

```csharp
mvvmNavigationManager.NavigateTo<FetchDataViewModel>();
```

You can also pass route segments or query strings through `relativeUri`.

## Navigate by abstraction

If your app depends on interfaces or abstract ViewModel types, you can navigate through those abstractions:

```csharp
mvvmNavigationManager.NavigateTo<ITestNavigationViewModel>();
```

The same pattern works in markup:

```razor
<MvvmNavLink class="nav-link"
             TViewModel=ITestNavigationViewModel
             Match="NavLinkMatch.All">
    <span class="oi oi-calculator" aria-hidden="true"></span>Test
</MvvmNavLink>
```

You can also append route data or query strings:

```razor
<MvvmNavLink class="nav-link"
             TViewModel=ITestNavigationViewModel
             RelativeUri="?test=this%20is%20a%20MvvmNavLink%20querystring%20test"
             Match="NavLinkMatch.All">
    <span class="oi oi-calculator" aria-hidden="true"></span>Test + QueryString
</MvvmNavLink>
```

## Navigate by key

If you register keyed ViewModels, you can navigate by string key:

```csharp
MvvmNavigationManager.NavigateTo("FetchDataViewModel");
```

Use [`MvvmKeyNavLink`](xref:Blazing.Mvvm.Components.Routing.MvvmKeyNavLink) for keyed navigation in Razor:

```razor
<MvvmKeyNavLink class="nav-link"
                NavigationKey="@nameof(TestKeyedNavigationViewModel)"
                Match="NavLinkMatch.All">
    <span class="oi oi-calculator" aria-hidden="true"></span> Keyed Test
</MvvmKeyNavLink>
```

`RelativeUri` works here too for route data and query strings.

## Fallback behavior

[`MvvmNavigationManager`](xref:Blazing.Mvvm.Components.MvvmNavigationManager) still supports normal `NavigationManager` string navigation internally, so you can mix MVVM-style navigation with standard Blazor routing where needed.

## Related topics

- [Route Patterns](route-patterns.md)
- [View Models](../configuration/view-models.md)
- [Sample Projects](../samples/sample-projects.md)
