using Blazing.Mvvm.Analyzers.Analyzers;
using Blazing.Mvvm.Analyzers.CodeFixProviders;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpCodeFixVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.EventCallbackTwoWayBindingAnalyzer,
    Blazing.Mvvm.Analyzers.CodeFixProviders.EventCallbackTwoWayBindingCodeFixProvider>;

namespace Blazing.Mvvm.Analyzers.Tests.CodeFixTests;

/// <summary>
/// Unit tests for <see cref="EventCallbackTwoWayBindingCodeFixProvider"/>.
/// </summary>
public class EventCallbackTwoWayBindingCodeFixProviderTests
{
    [Fact]
    public async Task MissingEventCallback_AddsCallbackParameter()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmComponentBase<TestViewModel>
{
    [Parameter]
    public int {|#0:Counter|} { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        const string fixedCode = @"
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmComponentBase<TestViewModel>
{
    [Parameter]
    public int Counter { get; set; }
    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback)
            .WithLocation(0)
            .WithArguments("Counter", "CounterChanged");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }

    [Fact]
    public async Task TypeMismatch_FixesEventCallbackGenericArgument()
    {
        const string test = @"
using System.ComponentModel;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmLayoutComponentBase<TestViewModel>
{
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<string> {|#0:CounterChanged|} { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public string Counter { get; set; }
}";

        const string fixedCode = @"
using System.ComponentModel;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmLayoutComponentBase<TestViewModel>
{
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public string Counter { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingTypeMismatch)
            .WithLocation(0)
            .WithArguments("CounterChanged", "int", "Counter");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }

    [Fact]
    public async Task ManualPattern_RemovesCanonicalManualBindingCode()
    {
        const string test = @"
using System.ComponentModel;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmOwningComponentBase<TestViewModel>
{
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        {|#0:ViewModel.PropertyChanged += OnViewModelPropertyChanged;|}
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Counter) && Counter != ViewModel.Counter)
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

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        const string fixedCode = @"
using System.ComponentModel;
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmOwningComponentBase<TestViewModel>
{
    [Parameter]
    public int Counter { get; set; }

    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern)
            .WithLocation(0)
            .WithArguments("Counter");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }
}
