# Introduction

**Blazing.Mvvm** brings MVVM patterns to Blazor through tight integration with [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/). It helps you build Blazor applications with ViewModel-first navigation, automatic ViewModel discovery and registration, parameter resolution between views and ViewModels, validation support, and reusable base components for common Blazor hosting models.

## What Blazing.Mvvm gives you

- ViewModel-aware base components such as [`MvvmComponentBase`](xref:Blazing.Mvvm.Components.MvvmComponentBase`1), [`MvvmOwningComponentBase`](xref:Blazing.Mvvm.Components.MvvmOwningComponentBase`1), and [`MvvmLayoutComponentBase`](xref:Blazing.Mvvm.Components.MvvmLayoutComponentBase`1)
- Base classes such as [`ViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ViewModelBase), [`RecipientViewModelBase`](xref:Blazing.Mvvm.ComponentModel.RecipientViewModelBase), and [`ValidatorViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ValidatorViewModelBase)
- Strongly typed navigation with [`MvvmNavigationManager`](xref:Blazing.Mvvm.Components.MvvmNavigationManager), [`MvvmNavLink`](xref:Blazing.Mvvm.Components.Routing.MvvmNavLink`1), and [`MvvmKeyNavLink`](xref:Blazing.Mvvm.Components.Routing.MvvmKeyNavLink)
- Parameter resolution and automatic two-way binding support
- Validation support through [`MvvmObservableValidator`](xref:Blazing.Mvvm.Components.MvvmObservableValidator)
- Sample applications that show the library across multiple hosting models

## Supported hosting models

Blazing.Mvvm supports:

- Blazor Server
- Blazor WebAssembly
- Blazor Web App
- Blazor Hybrid for WPF, WinForms, MAUI, and Avalonia

## Package lineup

Depending on what you need, the repository ships multiple packages:

- **Blazing.Mvvm** for the main MVVM integration, components, navigation, and helpers
- **Blazing.Mvvm.Base** for shared primitives and foundational building blocks
- **Blazing.Mvvm.Analyzers** for analyzer rules that help keep ViewModel usage and registration consistent

## Documentation map

Start with the page that matches your goal:

- **First-time setup:** [Getting Started](getting-started/quick-start.md)
- **Understand base classes and registration:** [View Models](configuration/view-models.md)
- **Wire component parameters to ViewModels:** [Parameter Resolution and Two-Way Binding](configuration/parameter-resolution.md)
- **Navigate by ViewModel instead of route strings:** [MVVM Navigation](navigation/mvvm-navigation.md)
- **Enable validation in forms:** [MVVM Validation](validation/mvvm-validation.md)
- **Run behind subpaths or reverse proxies:** [Subpath Hosting](hosting/subpath-hosting.md)
- **Browse examples:** [Sample Projects](samples/sample-projects.md)

## Repository reference

The repository `README.md` remains the broad project overview and quick reference:

- [Repository README](https://github.com/gragra33/Blazing.Mvvm/blob/develop/README.md)
- [Version History](https://github.com/gragra33/Blazing.Mvvm/blob/master/docs/archive/HISTORY.md)

## Next steps

1. Install the package and wire up services in [Getting Started](getting-started/quick-start.md).
2. Review [View Models](configuration/view-models.md) to choose the right base class and registration pattern.
3. Move to [MVVM Navigation](navigation/mvvm-navigation.md) and [MVVM Validation](validation/mvvm-validation.md) as your app grows.
