# BLAZMVVM0017: RelayCommand Async Pattern

## Diagnostic Information

- **Diagnostic ID**: BLAZMVVM0017
- **Category**: Blazing.Mvvm
- **Severity**: Warning
- **Title**: Async RelayCommand method should return Task

## Description

An asynchronous method marked with CommunityToolkit.Mvvm's `[RelayCommand]` attribute should return `Task`, not `void`. Returning `Task` lets the generated command use `AsyncRelayCommand`, observe exceptions, and expose asynchronous execution state.

The analyzer matches the CommunityToolkit attribute by its full type name, so unrelated attributes named `RelayCommand` do not trigger the rule.

## Examples

### ❌ Incorrect

```csharp
[RelayCommand]
private async void Save() // BLAZMVVM0017
{
    await SaveChangesAsync();
}
```

### ✅ Correct

```csharp
[RelayCommand]
private async Task Save()
{
    await SaveChangesAsync();
}
```

## Code Fix

The code fix changes the return type from `void` to `Task` and adds `using System.Threading.Tasks` when needed. It leaves the method name unchanged so references and the generated command name remain stable.

## Related

- **[BLAZMVVM0012](BLAZMVVM0012.md)**: Command Pattern
