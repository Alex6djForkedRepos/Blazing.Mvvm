# Route Patterns

Blazing.Mvvm supports route resolution patterns that work with both type-based navigation ([`NavigateTo<TViewModel>`](xref:Blazing.Mvvm.Components.IMvvmNavigationManager.NavigateTo*)) and keyed navigation (`NavigateTo(key)`).

## Simple routes

Use type-based navigation for static routes:

```csharp
mvvmNavigationManager.NavigateTo<HomeViewModel>();
mvvmNavigationManager.NavigateTo<CounterViewModel>();
mvvmNavigationManager.NavigateTo<FetchDataViewModel>();
```

## Single parameter routes

Pass one route parameter through `relativeUri`:

```csharp
mvvmNavigationManager.NavigateTo<UserViewModel>("123");
// /users/123

mvvmNavigationManager.NavigateTo<ProductViewModel>("abc-456");
// /products/abc-456
```

## Multiple parameter routes

Separate parameters with `/` in the same order as the route template:

```csharp
mvvmNavigationManager.NavigateTo<UserPostViewModel>("1/101");
// /users/1/posts/101

mvvmNavigationManager.NavigateTo<ApiUserPostViewModel>("v2/1/101");
// /api/v2/users/1/posts/101
```

## Query strings

Append query strings directly:

```csharp
mvvmNavigationManager.NavigateTo<ProductsViewModel>("?category=electronics");
mvvmNavigationManager.NavigateTo<SearchViewModel>("?query=blazor&sort=relevance&page=1");
```

## Combined route data and query strings

You can combine route parameters and query strings in the same call:

```csharp
mvvmNavigationManager.NavigateTo<UserViewModel>("123?tab=profile&edit=true");

mvvmNavigationManager.NavigateTo<UserPostViewModel>("1/101?filter=recent&sort=desc");

mvvmNavigationManager.NavigateTo<ApiUserPostViewModel>("v2/1/101?include=comments&expand=author");
```

## Complex multi-level routes

Nested route templates also work:

```csharp
mvvmNavigationManager.NavigateTo<UserPermissionsViewModel>("123");
// /admin/settings/users/123/permissions

mvvmNavigationManager.NavigateTo<ProjectViewModel>("abc/ws-123/proj-456");
// /app/tenant/abc/workspace/ws-123/project/proj-456
```

## Rules to keep in mind

- Parameters are substituted in route-template order
- Query strings start with `?` and multiple values are separated with `&`
- URL encoding is handled automatically
- Route resolution works with subpath hosting and YARP scenarios

## Related topics

- [MVVM Navigation](mvvm-navigation.md)
- [Subpath Hosting](../hosting/subpath-hosting.md)
- [Sample Projects](../samples/sample-projects.md)
