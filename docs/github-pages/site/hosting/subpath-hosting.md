# Subpath Hosting

Blazing.Mvvm supports hosting Blazor applications under a subpath such as `https://example.com/myapp` instead of the domain root.

## Automatic base path detection

Since v3.1.0, Blazing.Mvvm automatically detects the base path from `NavigationManager.BaseUri`. In many cases, including YARP reverse proxy setups, you do not need to configure `BasePath` manually.

Automatic detection works well for:

- standard subpath hosting
- YARP reverse proxy scenarios
- multi-tenant setups with dynamic paths
- development and production environments without different MVVM configuration

## Standard subpath hosting

### 1. Configure `launchSettings.json`

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "fu/bar",
      "applicationUrl": "https://localhost:7037;http://localhost:5272",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### 2. Configure middleware in `Program.cs`

```csharp
app.UsePathBase("/fu/bar/");
app.UseRouting();
```

### 3. Set the base href dynamically

For legacy `_Host.cshtml`:

```razor
@{
    var baseHref = HttpContext?.Request?.PathBase.HasValue == true
        ? HttpContext?.Request.PathBase.Value!.TrimEnd('/') + "/"
        : "/";
}

<base href="@baseHref" />
```

For `App.razor`:

```razor
@code {
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string baseHref => HttpContext?.Request.PathBase.HasValue == true
        ? HttpContext.Request.PathBase.Value!.TrimEnd('/') + "/"
        : "/";
}
```

### 4. Register Blazing.Mvvm normally

```csharp
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.Server;
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
});
```

## YARP support

Blazing.Mvvm also works when YARP sets `PathBase` on incoming requests.

### Configure YARP routes

```json
{
  "ReverseProxy": {
    "Routes": {
      "blazor-route": {
        "ClusterId": "blazor-cluster",
        "Match": {
          "Path": "/fu/bar/{**catch-all}"
        },
        "Transforms": [
          { "PathRemovePrefix": "/fu/bar" }
        ]
      }
    },
    "Clusters": {
      "blazor-cluster": {
        "Destinations": {
          "blazor-destination": {
            "Address": "http://localhost:5005/"
          }
        }
      }
    }
  }
}
```

### Configure forwarded headers and optional prefix handling

```csharp
app.UseForwardedHeaders();

app.Use((ctx, next) =>
{
    if (ctx.Request.Headers.TryGetValue("X-Forwarded-Prefix", out StringValues prefix) &&
        !StringValues.IsNullOrEmpty(prefix))
    {
        var p = prefix.ToString();
        if (!string.IsNullOrEmpty(p))
        {
            ctx.Request.PathBase = p;
        }
    }

    return next();
});
```

For development, you can also force a path base:

```csharp
app.Use((ctx, next) =>
{
    ctx.Request.PathBase = "/fu/bar";
    return next();
});
```

## Legacy explicit configuration

If you need to override automatic detection, `BasePath` still works:

```csharp
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.Server;
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
    options.BasePath = "/fu/bar/";
});
```

## Resolution priority

Base path resolution follows this order:

1. Explicit `BasePath`
2. Dynamic detection from `NavigationManager.BaseUri`

## Working examples and further reading

- [Blazing.SubpathHosting.Server sample](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.SubpathHosting.Server)
- [Subpath hosting guidance](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.SubpathHosting.Server/Subpath_Hosting_Guidance.md)
- [ASP.NET Core path base middleware](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer#path-base)
- [YARP documentation](https://learn.microsoft.com/aspnet/core/fundamentals/servers/yarp/getting-started)

## Related topics

- [Route Patterns](../navigation/route-patterns.md)
- [MVVM Navigation](../navigation/mvvm-navigation.md)
