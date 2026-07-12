# BLAZMVVM0016: Messenger Registration Lifetime Analyzer

## Diagnostic Information

- **Diagnostic ID**: BLAZMVVM0016
- **Category**: Blazing.Mvvm
- **Severity**: Warning
- **Title**: Messenger registration without unregistration

## Description

Detects messenger registrations (Register calls) without corresponding unregistration (Unregister calls). In current Blazing.Mvvm versions, inheriting from `ViewModelBase` is not sufficient cleanup by itself; the ViewModel should unregister messages in `Dispose(bool disposing)` or use `RecipientViewModelBase`.

## Examples

### ❌ Incorrect

```csharp
public class MyViewModel : ViewModelBase
{
    public MyViewModel()
    {
        Messenger.Register<MyMessage>(this, HandleMessage); // ⚠️ Warning
    }
}
```

### ✅ Correct (With Dispose)

```csharp
public class MyViewModel : ViewModelBase
{
    public MyViewModel()
    {
        Messenger.Register<MyMessage>(this, HandleMessage);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Messenger.UnregisterAll(this);
        }

        base.Dispose(disposing);
    }
}
```

### ✅ Correct (Using RecipientViewModelBase)

```csharp
public class MyViewModel : RecipientViewModelBase
{
    protected override void OnActivated()
    {
        Messenger.Register<MyMessage>(this, HandleMessage);
        // Automatically unregistered on deactivation
    }
}
```

## Related

- **[BLAZMVVM0015](BLAZMVVM0015.md)**: Dispose Pattern Analyzer
