# BLAZMVVM0007: Lifecycle Method Override

## Diagnostic Information

- **Diagnostic ID**: BLAZMVVM0007
- **Category**: Blazing.Mvvm
- **Severity**: Info
- **Title**: Lifecycle method should use override

## Description

A lifecycle method declared on a [`ViewModelBase`](xref:Blazing.Mvvm.ComponentModel.ViewModelBase)-derived type must override the matching virtual base method. A same-signature method without `override` only hides the base implementation, so the Blazing.Mvvm lifecycle does not dispatch to it.

The rule recognizes `OnInitialized`, `OnInitializedAsync`, `OnParametersSet`, `OnParametersSetAsync`, `OnAfterRender`, `OnAfterRenderAsync`, and `ShouldRender`. It reports only when the complete signature matches a virtual base method. An explicit `new` modifier is treated as intentional hiding and is not reported.

## Examples

### ❌ Incorrect

```csharp
public class ProductViewModel : ViewModelBase
{
    public Task OnInitializedAsync() // BLAZMVVM0007
    {
        return LoadProductsAsync();
    }
}
```

### ✅ Correct

```csharp
public class ProductViewModel : ViewModelBase
{
    public override Task OnInitializedAsync()
    {
        return LoadProductsAsync();
    }
}
```

## Code Fix

The code fix adds `override`. If the hiding method was declared `virtual`, the fix replaces `virtual` with `override`.

## Related

- **[BLAZMVVM0001](BLAZMVVM0001.md)**: ViewModelBase Inheritance
