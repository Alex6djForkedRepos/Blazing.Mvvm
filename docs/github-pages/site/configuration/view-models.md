# View Models

Blazing.Mvvm provides base classes and conventions that let Blazor components work with ViewModels in a predictable MVVM style.

## Available base classes

Use the base class that matches your ViewModel needs:

- [`ViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ViewModelBase) derives from `ObservableObject`
- [`RecipientViewModelBase`](xref:Blazing.Mvvm.ComponentModel.RecipientViewModelBase) derives from `ObservableRecipient`
- [`ValidatorViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ValidatorViewModelBase) derives from `ObservableValidator`

## Lifecycle methods

The ViewModel base classes mirror common `ComponentBase` lifecycle hooks:

- `OnInitialized`
- `OnInitializedAsync`
- `OnParametersSet`
- `OnParametersSetAsync`
- `OnAfterRender`
- `OnAfterRenderAsync`
- `ShouldRender`

This lets component lifecycle flow into the ViewModel without custom plumbing in every page.

## Disposal behavior

Since v3.2.1, all ViewModel base classes implement `IDisposable`. When a ViewModel is disposed, it automatically unsubscribes from all `IAsyncRelayCommand` `PropertyChanged` events. This matters most for commands with `AllowConcurrentExecutions` set to `false`, where the framework monitors the command's `IsRunning` property to trigger UI updates. Without cleanup, those subscriptions leak.

Automatic disposal gives you:

- **Memory leak prevention**: command event subscriptions are cleaned up without manual tracking
- **Simpler ViewModels**: no need to track and unsubscribe from command events yourself
- **A consistent pattern**: all ViewModels follow the standard .NET dispose pattern
- **Efficient garbage collection**: commands and ViewModels are released promptly

If a derived ViewModel needs to release additional resources, override `Dispose(bool disposing)`:

```csharp
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed partial class MyViewModel : ViewModelBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        base.Dispose(disposing);
    }
}
```

> [!WARNING]
> If you previously implemented `public void Dispose()` yourself, change that to `protected override void Dispose(bool disposing)` so the base class can keep its cleanup behavior.

## Service registration

ViewModels are registered as transient services by default. Use [`ViewModelDefinition`](xref:Blazing.Mvvm.ComponentModel.ViewModelDefinitionAttribute) to choose another lifetime:

```csharp
[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public partial class FetchDataViewModel : ViewModelBase
{
    // ViewModel code
}
```

Then inherit your component from the matching MVVM base type:

```razor
@page "/fetchdata"
@inherits MvvmComponentBase<FetchDataViewModel>
```

## Register with interfaces or abstract types

Use the generic [`ViewModelDefinition`](xref:Blazing.Mvvm.ComponentModel.ViewModelDefinitionAttribute) attribute when you want the ViewModel resolved through an abstraction:

```csharp
[ViewModelDefinition<IFetchDataViewModel>]
public partial class FetchDataViewModel : ViewModelBase, IFetchDataViewModel
{
    // ViewModel code
}
```

The component can then depend on that abstraction:

```razor
@page "/fetchdata"
@inherits MvvmComponentBase<IFetchDataViewModel>
```

## Register keyed ViewModels

Use a key when you need explicit string-based lookup:

```csharp
[ViewModelDefinition(Key = "FetchDataViewModel")]
public partial class FetchDataViewModel : ViewModelBase
{
    // ViewModel code
}
```

Reference the key on the component with `ViewModelKey`:

```razor
@page "/fetchdata"
@attribute [ViewModelKey("FetchDataViewModel")]
@inherits MvvmComponentBase<FetchDataViewModel>
```

## When to use each component base type

- [`MvvmComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmComponentBase`1): default choice for most pages and components
- [`MvvmOwningComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmOwningComponentBase`1): use when the component needs its own scoped dependency lifetime
- [`MvvmLayoutComponentBase<TViewModel>`](xref:Blazing.Mvvm.Components.MvvmLayoutComponentBase`1): use when the layout itself owns a ViewModel

## Related topics

- [Parameter Resolution and Two-Way Binding](parameter-resolution.md)
- [Multi-Project ViewModel Registration](multi-project-registration.md)
- [MVVM Navigation](../navigation/mvvm-navigation.md)
