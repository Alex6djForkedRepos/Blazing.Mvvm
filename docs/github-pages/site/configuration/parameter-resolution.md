# Parameter Resolution and Two-Way Binding

Blazing.Mvvm can flow values from a view into its ViewModel and automatically wire two-way binding patterns that follow normal Blazor conventions.

## Parameter resolution

Parameter resolution is opt-in. Configure it when registering the library:

```csharp
builder.Services.AddMvvm(options =>
{
    options.ParameterResolutionMode = ParameterResolutionMode.ViewAndViewModel;
});
```

Use:

- [`ParameterResolutionMode.ViewAndViewModel`](xref:Blazing.Mvvm.ParameterResolutionMode.ViewAndViewModel) to resolve parameters in both the Razor component and the ViewModel
- [`ParameterResolutionMode.ViewModel`](xref:Blazing.Mvvm.ParameterResolutionMode.ViewModel) to resolve parameters in the ViewModel only

## Mark ViewModel properties with `ViewParameter`

Properties in the ViewModel that should receive values from the view must be marked with [`ViewParameter`](xref:Blazing.Mvvm.ComponentModel.ViewParameterAttribute):

```csharp
public partial class SampleViewModel : ViewModelBase
{
    [ObservableProperty]
    [property: ViewParameter]
    private string _title = default!;

    [ObservableProperty]
    [property: ViewParameter("Count")]
    private int _counter;

    [ViewParameter]
    public string? Content { get; set; }
}
```

## Define matching component parameters

In the Razor component, define matching Blazor parameters:

```razor
@page "/sample"
@inherits MvvmComponentBase<SampleViewModel>

@code {
    [Parameter]
    public string Title { get; set; } = default!;

    [Parameter]
    public int Count { get; set; }

    [Parameter]
    public string? Content { get; set; }
}
```

## Automatic two-way binding

Since v3.2.0, Blazing.Mvvm can automatically wire common `@bind-` patterns. This removes the need to manually subscribe to ViewModel changes and forward values through `EventCallback<T>`.

Automatic wiring happens when:

- the component exposes an `EventCallback<T>` named with Blazor's `{PropertyName}Changed` convention
- the ViewModel exposes a matching [`[ViewParameter]`](xref:Blazing.Mvvm.ComponentModel.ViewParameterAttribute) property

### ViewModel

```csharp
public partial class CounterComponentViewModel : ViewModelBase
{
    [ObservableProperty]
    [property: ViewParameter]
    private int _counter;
}
```

### Component

```razor
@inherits MvvmComponentBase<CounterComponentViewModel>

<p role="status">Current count: <strong>@ViewModel.Counter</strong></p>

@code {
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }
}
```

### Parent usage

```razor
<CounterComponent @bind-Counter="@ViewModel.Counter" />
```

### What this replaces

Before v3.2.0, the same component needed manual `PropertyChanged` event handling, around 30 lines of boilerplate per component:

```razor
@using System.ComponentModel
@inherits MvvmComponentBase<CounterComponentViewModel>

<p role="status">Current count: <strong>@ViewModel.Counter</strong></p>

@code {
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Counter) && ViewModel.Counter != Counter)
        {
            await CounterChanged.InvokeAsync(ViewModel.Counter);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        base.Dispose(disposing);
    }
}
```

With automatic two-way binding, the subscription, forwarding, and disposal above are handled by the framework. The component shrinks to just the two parameter declarations shown earlier.

## Why this matters

Automatic two-way binding gives you:

- **Zero configuration**: matching `EventCallback` parameters are detected and wired up during component initialization
- **Less boilerplate**: eliminates 20+ lines of event handling code per component
- **Safer cleanup**: subscriptions are disposed automatically when the component is removed, preventing memory leaks
- **Type-safe**: parameter types are checked at compile time
- **A pattern that still feels like normal Blazor**: follows the standard `@bind-` naming convention
- **Support across [`MvvmComponentBase`](xref:Blazing.Mvvm.Components.MvvmComponentBase`1), [`MvvmOwningComponentBase`](xref:Blazing.Mvvm.Components.MvvmOwningComponentBase`1), and [`MvvmLayoutComponentBase`](xref:Blazing.Mvvm.Components.MvvmLayoutComponentBase`1)**

> [!NOTE]
> For a complete working demonstration of parameter resolution and automatic two-way binding, see the **ParameterResolution** sample page included in most [Sample Projects](../samples/sample-projects.md).

## Related topics

- [View Models](view-models.md)
- [MVVM Navigation](../navigation/mvvm-navigation.md)
- [Sample Projects](../samples/sample-projects.md)
