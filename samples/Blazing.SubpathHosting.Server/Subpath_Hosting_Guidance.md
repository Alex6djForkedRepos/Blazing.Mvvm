# Blazor Subpath Hosting Configuration

This project demonstrates hosting a Blazor Server app under a subpath (`/fu/bar`).

## Configuration

### Program.cs
- `UsePathBase("/fu/bar")` must be called **BEFORE** other middleware
- No trailing slash on the path base
- Path base should be consistent across all configuration

### App.razor
- Dynamic `<base href>` generation from `HttpContext.Request.PathBase`
- Ensures all relative URLs are resolved correctly

## Common Issues and Solutions

### Issue: Subpath drops during navigation

**Symptoms:**
- URL changes from `http://localhost:7037/fu/bar/counter` to `http://localhost:7037/counter`
- Application still works when subpath is removed

**Causes and Fixes:**

1. **Forms with enhanced navigation**
   - ? Use `EditForm` component (handles path base automatically)
   - ? Avoid plain HTML `<form>` elements without proper action URLs

2. **Static file references**
   - ? Use relative paths: `css/site.css`
   - ? Avoid absolute paths: `/css/site.css`
   - The `<base href>` will automatically resolve relative paths

3. **Navigation links**
   - ? Use `NavLink` with relative href: `href="counter"`
   - ? Use `NavLink` with empty href for home: `href=""`
   - ? Avoid absolute paths: `href="/counter"`

4. **Programmatic navigation**
   - ? Use `IMvvmNavigationManager.NavigateTo<T>()`
   - ? Use standard `NavigationManager.NavigateTo()` with relative URLs
   - The framework handles path base automatically

5. **JavaScript/Browser APIs**
   - ?? Custom JavaScript using `window.location` or `history.pushState` needs manual path base handling
   - Use NavigationManager from .NET instead when possible

## Testing

To test subpath hosting:

1. Run the application
2. Navigate to: `http://localhost:7037/fu/bar/` (note trailing slash for root)
3. Click through navigation - URLs should maintain `/fu/bar` prefix
4. Check browser DevTools Network tab for correct resource URLs

## Deployment Considerations

When deploying behind a reverse proxy (nginx, IIS, etc.):

### IIS
```xml
<system.webServer>
  <aspNetCore processPath="dotnet" 
              arguments=".\Blazing.SubpathHosting.Server.dll" 
              stdoutLogEnabled="false">
    <environmentVariables>
      <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
    </environmentVariables>
  </aspNetCore>
</system.webServer>
```

Configure the application pool to use "/fu/bar" as the virtual path.

### nginx
```nginx
location /fu/bar {
    proxy_pass http://localhost:5000;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection keep-alive;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_cache_bypass $http_upgrade;
}
```

### Docker with environment variable
```dockerfile
ENV ASPNETCORE_PATHBASE=/fu/bar
```

Or configure in appsettings.json:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  },
  "PathBase": "/fu/bar"
}
```

Then in Program.cs:
```csharp
var pathBase = builder.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
```
