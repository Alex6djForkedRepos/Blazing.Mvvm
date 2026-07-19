# Blazing.Mvvm.Analyzers

Roslyn analyzers for [Blazing.Mvvm](https://github.com/gragra33/Blazing.Mvvm) to help developers follow MVVM best practices and catch common mistakes at compile time.

## Installation

The analyzers are available as a **separate, optional NuGet package**. Install it alongside Blazing.Mvvm:

```bash
dotnet add package Blazing.Mvvm
dotnet add package Blazing.Mvvm.Analyzers
```

Or via Package Manager Console:

```powershell
Install-Package Blazing.Mvvm
Install-Package Blazing.Mvvm.Analyzers
```

> **Note**: The analyzers package is **optional**. You can use Blazing.Mvvm without the analyzers if you prefer.

## Analyzers

This package includes **21 analyzers** to help you write better Blazing.Mvvm code:

### Phase 1: Core MVVM Pattern (High Priority)

- **[BLAZMVVM0001](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0001.html)**: ViewModelBase Inheritance - Ensures ViewModels inherit from the correct base class
- **[BLAZMVVM0002](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0002.html)**: ViewModelDefinition Attribute - Ensures proper ViewModel registration for DI
- **[BLAZMVVM0003](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0003.html)**: MvvmComponentBase Usage - Ensures proper View-ViewModel binding in components
- **[BLAZMVVM0005](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0005.html)**: Navigation Type Safety - Validates NavigateTo<TViewModel> calls reference valid routes
- **[BLAZMVVM0013](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0013.html)**: MvvmOwningComponentBase Usage - Detects when scoped services require owned scope

### Phase 2: Best Practices (Medium Priority)

- **[BLAZMVVM0004](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0004.html)**: ViewParameter Attribute - Validates ViewParameter and Parameter property matching
- **[BLAZMVVM0007](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0007.html)**: Lifecycle Method Override - Ensures ViewModel lifecycle methods override their virtual base methods
- **[BLAZMVVM0008](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0008.html)**: Observable Property - Ensures proper usage of [ObservableProperty] and SetProperty
- **[BLAZMVVM0015](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0015.html)**: Dispose Pattern - Detects ViewModels requiring IDisposable implementation
- **[BLAZMVVM0016](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0016.html)**: Messenger Registration Lifetime - Detects messenger registrations without cleanup
- **[BLAZMVVM0017](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0017.html)**: RelayCommand Async Pattern - Ensures asynchronous RelayCommand methods return Task
- **[BLAZMVVM0018](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0018.html)**: NotifyPropertyChangedFor - Suggests notifications for computed properties
- **[BLAZMVVM0020](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0020.html)**: Route Parameter Binding - Validates route parameters have corresponding properties
- **[BLAZMVVM0021](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0021.html)**: EventCallback Two-Way Binding - Detects obsolete manual bindings, missing callbacks, and callback type mismatches

### Phase 3: Code Quality (Info Level)

- **[BLAZMVVM0010](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0010.html)**: Route-ViewModel Mapping - Ensures Pages have corresponding ViewModels
- **[BLAZMVVM0012](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0012.html)**: Command Pattern - Encourages proper [RelayCommand] usage over public methods
- **[BLAZMVVM0014](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0014.html)**: StateHasChanged Overuse - Detects unnecessary StateHasChanged() calls
- **[BLAZMVVM0019](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0019.html)**: CascadingParameter vs Inject - Suggests [Inject] for DI services

### Phase 4: Advanced (Specialized)

- **[BLAZMVVM0006](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0006.html)**: ViewModelKey Consistency - Ensures ViewModelKey values match navigation keys
- **[BLAZMVVM0009](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0009.html)**: Service Injection - Flags `[Inject]` properties in ViewModels and recommends constructor injection
- **[BLAZMVVM0011](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/BLAZMVVM0011.html)**: MvvmNavLink Type Safety - Validates MvvmNavLink TViewModel parameter

## Code Fix Providers

The package includes **14 code fix providers** for automatic corrections:

### Core MVVM Pattern Fixes

1. **ViewModelBaseInheritanceCodeFixProvider** - Adds ViewModelBase inheritance
2. **ViewModelDefinitionAttributeCodeFixProvider** - Adds [ViewModelDefinition] attribute
3. **MvvmComponentBaseUsageCodeFixProvider** - Replaces ComponentBase with MvvmComponentBase<TViewModel>
4. **MvvmOwningComponentBaseUsageCodeFixProvider** - Replaces MvvmComponentBase with MvvmOwningComponentBase

### Best Practices Fixes

1. **RouteParameterBindingCodeFixProvider** - Generates missing [Parameter] or [ViewParameter] properties
2. **DisposePatternCodeFixProvider** - Adds IDisposable implementation with cleanup
3. **MessengerRegistrationLifetimeCodeFixProvider** - Adds Dispose with Unregister or OnActivated pattern
4. **NotifyPropertyChangedForCodeFixProvider** - Adds [NotifyPropertyChangedFor] attribute
5. **LifecycleMethodOverrideCodeFixProvider** - Adds the override modifier to lifecycle methods
6. **RelayCommandAsyncCodeFixProvider** - Changes async RelayCommand methods to return Task

### Code Quality Fixes

1. **CommandPatternCodeFixProvider** - Adds [RelayCommand] attribute and makes method private
2. **StateHasChangedOveruseCodeFixProvider** - Removes unnecessary StateHasChanged() calls
3. **CascadingParameterVsInjectCodeFixProvider** - Replaces [CascadingParameter] with [Inject]
4. **EventCallbackTwoWayBindingCodeFixProvider** - Removes canonical manual two-way binding code and fixes EventCallback two-way binding parameters

## Severity Levels

- **Error**: Must be fixed (BLAZMVVM0003, 0005, 0011)
- **Warning**: Should be addressed (BLAZMVVM0001, 0002, 0004, 0006, 0008, 0009, 0013, 0015, 0016, 0017, 0020)
- **Info**: Consider improvements (BLAZMVVM0007, 0010, 0012, 0014, 0018, 0019, and the manual/missing-callback patterns of BLAZMVVM0021)
- **Warning**: The type-mismatch pattern of BLAZMVVM0021 is reported as a Warning when the EventCallback generic argument does not match the component parameter type

## Quick Start

After installing the package, analyzers will automatically run during compilation. Look for diagnostic messages starting with "BLAZMVVM" in your IDE:

```csharp
// ⚠ Warning BLAZMVVM0001
public class MyViewModel // Missing base class
{
}

// ✓ Correct
public class MyViewModel : ViewModelBase
{
}
```

Many diagnostics include quick fixes available via the lightbulb icon (💡) or `Ctrl+.` shortcut.

## Usage Examples

### In Sample Projects

To use the analyzers in sample projects, add a package reference:

```xml
<ItemGroup>
  <PackageReference Include="Blazing.Mvvm.Analyzers" Version="*" />
</ItemGroup>
```

Or use a project reference during development:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\Libraries\Blazing.Mvvm.Analyzers\Blazing.Mvvm.Analyzers.csproj" 
                    PrivateAssets="all" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

### Disabling Specific Analyzers

If you want to disable specific analyzers, add them to your `.editorconfig`:

```ini
# Disable Command Pattern analyzer
dotnet_diagnostic.BLAZMVVM0012.severity = none
```

## Documentation

For detailed information about each analyzer, click the links above or visit the [analyzers documentation folder](https://gragra33.github.io/Blazing.Mvvm/site/analyzers/index.html).

For more information about Blazing.Mvvm, see the [main documentation](https://github.com/gragra33/Blazing.Mvvm).

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## License

MIT License - see the LICENSE file for details.
