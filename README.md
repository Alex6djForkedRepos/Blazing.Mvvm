<p>
  <img src="https://raw.githubusercontent.com/gragra33/Blazing.Mvvm/master/docs/github-pages/images/logo.png" alt="Blazing.Mvvm logo" width="96">
</p>

# Blazing.Mvvm

[![CI/CD](https://github.com/gragra33/Blazing.Mvvm/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/gragra33/Blazing.Mvvm/actions/workflows/ci-cd.yml)

[![Blazing.Mvvm](https://img.shields.io/nuget/v/Blazing.Mvvm?logo=nuget&label=Blazing.Mvvm)](https://www.nuget.org/packages/Blazing.Mvvm) [![Blazing.Mvvm.Analyzers](https://img.shields.io/nuget/v/Blazing.Mvvm.Analyzers?logo=nuget&label=Blazing.Mvvm.Analyzers)](https://www.nuget.org/packages/Blazing.Mvvm.Analyzers) [![Downloads](https://img.shields.io/nuget/dt/Blazing.Mvvm?logo=nuget&label=Downloads)](https://www.nuget.org/packages/Blazing.Mvvm)

🔥 **Blazing.Mvvm** brings full MVVM support to Blazor through seamless integration with the [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/). It provides strongly-typed, ViewModel-first navigation, automatic ViewModel registration and discovery, parameter resolution between Views and ViewModels, validation with `ObservableValidator`, and comprehensive lifecycle management across every Blazor hosting model (Server, WebAssembly, Static SSR, Auto, Hybrid, and MAUI). It also ships Roslyn analyzers with one-click code fixes to catch common MVVM mistakes at build time.

## Quick Start

**1. Install the package**

```bash
dotnet add package Blazing.Mvvm
```

**2. Register Blazing.Mvvm in `Program.cs`**

```csharp
builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.WebApp;
});
```

**3. Create a ViewModel**

```csharp
public partial class CounterViewModel : ViewModelBase
{
    [ObservableProperty]
    private int _count;

    [RelayCommand]
    private void Increment() => Count++;
}
```

**4. Use it in a component**

```razor
@page "/counter"
@inherits MvvmComponentBase<CounterViewModel>

<p role="status">Current count: @ViewModel.Count</p>
<button class="btn btn-primary" @onclick="ViewModel.IncrementCommand.Execute">
    Click me
</button>
```

For the full walkthrough, including configuration options, ViewModel setup, and component usage, see the [Getting Started](https://gragra33.github.io/Blazing.Mvvm/site/getting-started/quick-start.html) guide.

## Documentation

Full documentation, guides, samples, analyzer rules, and the API reference are available on the [documentation site](https://gragra33.github.io/Blazing.Mvvm/).

## Share Your Feedback

If you like the library, use it, share it, and give it a ⭐️. For any suggestions, feature requests, or issues, feel free to create an [issue](https://github.com/gragra33/Blazing.Mvvm/issues) to help improve the library.

## Contributing

Refer to the [Contributing](https://github.com/gragra33/Blazing.Mvvm/blob/master/CONTRIBUTING.md) guide for more details.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/gragra33/Blazing.Mvvm/blob/master/LICENSE) file for details.
