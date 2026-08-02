# Multi-Project ViewModel Registration

In larger solutions, ViewModels are often split across multiple assemblies. This is common in hybrid applications where UI, shared components, and business logic live in different projects.

Blazing.Mvvm supports explicit multi-assembly registration so navigation and component resolution still work across the full solution.

## Register ViewModels from multiple assemblies

Use [`RegisterViewModelsFromAssemblyContaining`](xref:Blazing.Mvvm.LibraryConfiguration.RegisterViewModelsFromAssemblyContaining*) inside [`AddMvvm`](xref:Blazing.Mvvm.ServicesExtension.AddMvvm*):

```csharp
using Blazing.Mvvm;
using HybridSample.Core.ViewModels;
using HybridSample.Blazor.Core.Pages;

builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.Hybrid;

    options.RegisterViewModelsFromAssemblyContaining<SamplePageViewModel>();
    options.RegisterViewModelsFromAssemblyContaining<IntroductionPage>();
});
```

This works well when you know one representative type from each assembly you want scanned.

## Register assemblies directly

You can also register assemblies explicitly:

```csharp
var coreAssembly = typeof(SamplePageViewModel).Assembly;
var blazorAssembly = typeof(IntroductionPage).Assembly;

builder.Services.AddMvvm(options =>
{
    options.RegisterViewModelsFromAssembly(coreAssembly, blazorAssembly);
});
```

Or register a collection:

```csharp
var assemblies = new[] { coreAssembly, blazorAssembly };

builder.Services.AddMvvm(options =>
{
    options.RegisterViewModelsFromAssemblies(assemblies);
});
```

## When this is useful

This pattern is especially useful when you have:

- a core project with shared business logic and ViewModels
- a Blazor UI project with page-specific ViewModels
- shared UI libraries reused across multiple hosts
- hybrid applications that mix native shell projects with Blazor content

## Working examples

See the hybrid sample projects for real repository examples:

- [HybridSample.Wpf](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.Wpf)
- [HybridSample.WinForms](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.WinForms)
- [HybridSample.MAUI](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.MAUI)
- [HybridSample.Avalonia](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.Avalonia)

## Related topics

- [Getting Started](../getting-started/quick-start.md)
- [View Models](view-models.md)
- [Sample Projects](../samples/sample-projects.md)
