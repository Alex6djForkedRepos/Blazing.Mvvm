<p>
  <img src="https://raw.githubusercontent.com/gragra33/Blazing.Mvvm/master/docs/github-pages/images/logo.png" alt="Blazing.Mvvm logo" width="96">
</p>

# Blazing.Mvvm.Analyzers

[![CI/CD](https://github.com/gragra33/Blazing.Mvvm/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/gragra33/Blazing.Mvvm/actions/workflows/ci-cd.yml)

[![Blazing.Mvvm.Analyzers](https://img.shields.io/nuget/v/Blazing.Mvvm.Analyzers?logo=nuget&label=Blazing.Mvvm.Analyzers)](https://www.nuget.org/packages/Blazing.Mvvm.Analyzers) [![Downloads](https://img.shields.io/nuget/dt/Blazing.Mvvm.Analyzers?logo=nuget&label=Downloads)](https://www.nuget.org/packages/Blazing.Mvvm.Analyzers)

🔎 **Blazing.Mvvm.Analyzers** is the optional Roslyn analyzer package for [Blazing.Mvvm](https://github.com/gragra33/Blazing.Mvvm). It catches common MVVM mistakes at build time, including incorrect ViewModel inheritance, missing registration attributes, unsafe navigation, parameter mismatches, and incomplete cleanup patterns. Many diagnostics include a one-click code fix.

## Quick Start

**1. Install the packages**

```bash
dotnet add package Blazing.Mvvm
dotnet add package Blazing.Mvvm.Analyzers
```

**2. Build your project**

No additional registration or configuration is required. The analyzers run automatically in supported IDEs and during `dotnet build`.

```bash
dotnet build
```

**3. Review and fix diagnostics**

Diagnostics use the `BLAZMVVM` prefix and identify both the problem and the affected code:

```csharp
// BLAZMVVM0001: ViewModels should inherit from ViewModelBase.
public class CounterViewModel
{
}

// Correct
[ViewModelDefinition]
public partial class CounterViewModel : ViewModelBase
{
}
```

When a code fix is available, use the IDE lightbulb or `Ctrl+.` to apply it.

For the complete analyzer catalog, severity levels, configuration options, examples, and code-fix guidance, see the [Analyzer Rule Guides](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/index.html).

## Documentation

Full analyzer documentation is available in the [Analyzer Rule Guides](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/index.html). The generated analyzer types are covered in the [API reference](https://gragra33.github.io/Blazing.Mvvm/api-docs/blazing-mvvm-analyzers/Blazing.Mvvm.Analyzers.html).

## Share Your Feedback

If the analyzers help you, use them, share them, and give the project a ⭐️. For suggestions, feature requests, false positives, or issues, create an [issue](https://github.com/gragra33/Blazing.Mvvm/issues).

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/gragra33/Blazing.Mvvm/blob/master/LICENSE) file for details.
