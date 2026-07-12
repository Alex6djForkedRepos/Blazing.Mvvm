# Sample Projects

The repository includes sample projects that demonstrate Blazing.Mvvm across multiple hosting models and application shapes.

As of February 2, 2026, the main Blazor MVVM samples share common code through **`Blazing.Mvvm.Sample.Shared`**. This shows how to share ViewModels, services, components, and pages across several hosts.

Sample solution files include their local project-reference closure, so they can be opened and built reliably in Visual Studio while still validating the current repository source.

## Blazor hosting model samples

These samples use the shared project and focus on common Blazor hosting models:

- [Blazing.Mvvm.Sample.Server](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.Mvvm.Sample.Server)
- [Blazing.Mvvm.Sample.Wasm](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.Mvvm.Sample.Wasm)
- [Blazing.Mvvm.Sample.WebApp](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.Mvvm.Sample.WebApp)
- [Blazing.Mvvm.Sample.HybridMaui](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.Mvvm.Sample.HybridMaui)
- [Blazing.SubpathHosting.Server](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.SubpathHosting.Server)

## Shared sample content

`Blazing.Mvvm.Sample.Shared` includes examples for:

- relay commands, including async command behavior and `AllowConcurrentExecutions`
- parameter resolution and automatic two-way binding
- parent-child communication through messaging
- MVVM validation
- multi-parameter routing
- reusable Bootstrap-oriented components

## Hybrid samples

The repository also includes hybrid samples inspired by the CommunityToolkit MVVM samples:

- [HybridSample.Wpf](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.Wpf)
- [HybridSample.WinForms](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.WinForms)
- [HybridSample.MAUI](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.MAUI)
- [HybridSample.Avalonia](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/HybridSamples/HybridSample.Avalonia) (Windows only)

These modernize Microsoft's [Xamarin Sample](https://github.com/CommunityToolkit/MVVM-Samples) project for Blazing.Mvvm with minimal changes. The original standalone project, [Blazor MVVM Sample](https://github.com/gragra33/MvvmSampleBlazor), is now archived.

## Specialized samples

- [Blazing.SubpathHosting.Server](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/Blazing.SubpathHosting.Server) focuses on subpath hosting and reverse proxy scenarios.

## Archived or moved samples

Some older standalone samples were folded into the shared sample library:

- [ParameterResolution.Sample.Wasm](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/libs/Blazing.Mvvm.Sample.Shared/Pages/ParameterResolution)
- [Blazing.Mvvm.ParentChildSample](https://github.com/gragra33/Blazing.Mvvm/tree/master/samples/libs/Blazing.Mvvm.Sample.Shared/Pages/ParentChild)

## Component libraries used by samples

The samples also demonstrate reusable component libraries:

### `Blazing.Buttons`

- MVVM-aware button component
- command binding support
- disabled-state handling for commands

### `Blazing.Mvvm.Sample.Shared/Components/Bootstrap`

Production-ready Bootstrap 5 wrapper components demonstrating component composition patterns:

- `BootstrapAccordion` and `BootstrapAccordionItem`: collapsible content panels
- `BootstrapBreadcrumbs`: navigation breadcrumb trails with an MVVM-friendly API
- `BootstrapCard`: content containers with headers, footers, and customizable styling
- `BootstrapNavMenu` and `BootstrapNavMenuGroup`: hierarchical navigation menus with collapsible groups
- `BootstrapRowGroup` and `BootstrapRowGroupItem`: grouped row layouts for structured content

### `Blazing.Common`

Shared utility components and helpers used across the sample projects:

- `ConditionalSwitch`, `When`, `Otherwise`: declarative conditional rendering (an alternative to if/else in markup)
- `ComponentControlBase`, `ComponentInputControlBase`: base classes for building reusable components

## Running samples with different .NET versions

Most sample projects support multi-targeting across .NET 8, .NET 9, and .NET 10.

Typical workflow:

1. Open the sample solution in Visual Studio or your preferred IDE.
2. Set the sample project as the startup project.
3. Choose the target framework from the run configuration dropdown.
4. Start the application.

For detailed guidance, see [Running Different .NET Versions](running-different-net-versions.md).

## Related topics

- [Getting Started](../getting-started/quick-start.md)
- [Multi-Project ViewModel Registration](../configuration/multi-project-registration.md)
- [Subpath Hosting](../hosting/subpath-hosting.md)
