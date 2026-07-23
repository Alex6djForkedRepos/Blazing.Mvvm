# Blazing.Mvvm

[![NuGet Version](https://img.shields.io/nuget/v/Blazing.Mvvm.svg)](https://www.nuget.org/packages/Blazing.Mvvm)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Blazing.Mvvm.svg)](https://www.nuget.org/packages/Blazing.Mvvm)
[![.NET 8](https://img.shields.io/badge/.NET-8-512BD4)](https://dotnet.microsoft.com/download)
[![.NET 9](https://img.shields.io/badge/.NET-9-512BD4)](https://dotnet.microsoft.com/download)
[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)](https://dotnet.microsoft.com/download)

<div class="hero">
  <div class="hero-copy">
    <p class="hero-kicker">Blazor + CommunityToolkit.Mvvm</p>
    <p class="hero-title">Bring full MVVM patterns to Blazor without hand-written glue code.</p>
    <p>Blazing.Mvvm adds ViewModel-aware components, strongly typed navigation, parameter resolution, validation support, and sample apps across Server, WebAssembly, Web App, and Hybrid hosts.</p>
    <div class="hero-actions">
      <a class="doc-button primary" href="site/getting-started/quick-start.md">Get started</a>
      <a class="doc-button secondary" href="site/introduction.md">Learn the basics</a>
      <a class="doc-button secondary" href="https://www.nuget.org/packages/Blazing.Mvvm">NuGet</a>
    </div>
  </div>
</div>

## Quick Start

<div class="get-started">

1. **Install the package**

   ```bash
   dotnet add package Blazing.Mvvm
   ```

2. **Register Blazing.Mvvm in `Program.cs`**

   ```csharp
   builder.Services.AddMvvm(options =>
   {
       options.HostingModelType = BlazorHostingModelType.WebApp;
   });
   ```

3. **Create a ViewModel**

   ```csharp
   public partial class CounterViewModel : ViewModelBase
   {
       [ObservableProperty]
       private int _count;

       [RelayCommand]
       private void Increment() => Count++;
   }
   ```

4. **Use it in a component**

   ```razor
   @page "/counter"
   @inherits MvvmComponentBase<CounterViewModel>

   <p role="status">Current count: @ViewModel.Count</p>
   <button class="btn btn-primary" @onclick="ViewModel.IncrementCommand.Execute">
       Click me
   </button>
   ```

</div>

For the full walkthrough, including configuration options, ViewModel setup, and component usage, see the [Getting Started](site/getting-started/quick-start.md) guide.

## Highlights

<div class="bento">
  <div class="cell span2">
    <h4>ViewModel components</h4>
    <p>Inherit <code>MvvmComponentBase</code> for most pages and components. Use <code>MvvmOwningComponentBase</code> when the view needs its own DI scope for a <code>DbContext</code> or repository, or <code>MvvmLayoutComponentBase</code> for layouts. Lifecycle calls such as <code>OnInitializedAsync</code> run on the ViewModel.</p>
  </div>
  <div class="cell">
    <h4>Typed navigation</h4>
    <p>Call <code>NavigateTo&lt;TViewModel&gt;()</code> and the route is resolved from the type, an interface, or a key. Rename a <code>@page</code> and your call sites still compile.</p>
  </div>
  <div class="cell">
    <h4>Parameters and binding</h4>
    <p>Mark a property <code>[ViewParameter]</code> to receive the matching component <code>[Parameter]</code>. Components that follow the <code>@bind-</code> convention get two-way binding, cleaned up on dispose.</p>
  </div>
  <div class="cell">
    <h4>Validation</h4>
    <p><code>MvvmObservableValidator</code> plugs an <code>ObservableValidator</code> model into an <code>EditForm</code>, so <code>[Required]</code> and <code>[EmailAddress]</code> validate the usual way.</p>
  </div>
  <div class="cell">
    <h4>Messaging</h4>
    <p><code>RecipientViewModelBase</code> wraps <code>ObservableRecipient</code>, so ViewModels send and receive <code>IMessenger</code> messages for parent-child and cross-page updates.</p>
  </div>
  <div class="cell span2">
    <h4>Runs on every host</h4>
    <p>The same ViewModels run on Blazor Server, WebAssembly, Web App, and Hybrid (WPF, WinForms, MAUI, Avalonia). Subpath and YARP base paths are read from the request, so reverse-proxy setups need no extra configuration.</p>
  </div>
  <div class="cell">
    <h4>Analyzers and fixes</h4>
    <p>Roslyn analyzers flag the wrong base class, an unsafe navigation target, or a broken dispose, and ship with one-click code fixes.</p>
  </div>
</div>
