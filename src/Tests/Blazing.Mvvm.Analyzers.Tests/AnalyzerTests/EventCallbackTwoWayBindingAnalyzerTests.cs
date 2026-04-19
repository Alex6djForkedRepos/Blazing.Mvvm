using Blazing.Mvvm.Analyzers.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpAnalyzerVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.EventCallbackTwoWayBindingAnalyzer>;

namespace Blazing.Mvvm.Analyzers.Tests.AnalyzerTests;

/// <summary>
/// Unit tests for <see cref="EventCallbackTwoWayBindingAnalyzer"/>.
/// </summary>
public class EventCallbackTwoWayBindingAnalyzerTests
{
    [Fact]
    public async Task EmptyCode_NoDiagnostic()
    {
        await VerifyCS.VerifyAnalyzerAsync(string.Empty);
    }

    [Fact]
    public async Task MissingEventCallbackOnMvvmComponent_ReportsInfoDiagnostic()
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

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback)
            .WithLocation(0)
            .WithArguments("Counter", "CounterChanged");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task MissingEventCallbackOnLayoutComponent_ReportsInfoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestLayout : MvvmLayoutComponentBase<TestViewModel>
{
    [Parameter]
    public string {|#0:Title|} { get; set; }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public string Title { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback)
            .WithLocation(0)
            .WithArguments("Title", "TitleChanged");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task MatchingParameterCallbackAndViewParameter_NoDiagnostic()
    {
        const string test = @"
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

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task InheritedComponentCallbackAndViewModelViewParameter_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public abstract class BaseComponent : MvvmComponentBase<DerivedViewModel>
{
    [Parameter]
    public EventCallback<int> CounterChanged { get; set; }
}

public class DerivedComponent : BaseComponent
{
    [Parameter]
    public int Counter { get; set; }
}

public class BaseViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}

public class DerivedViewModel : BaseViewModel
{
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task EventCallbackTypeMismatch_UsesComponentParameterTypeAsSourceOfTruth()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Blazing.Mvvm.Components;
using Microsoft.AspNetCore.Components;

namespace TestNamespace;

public class TestComponent : MvvmComponentBase<TestViewModel>
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

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingTypeMismatch)
            .WithLocation(0)
            .WithArguments("CounterChanged", "int", "Counter");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CanonicalManualPattern_ReportsInfoDiagnostic()
    {
        const string test = @"
using System.ComponentModel;
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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        {|#0:ViewModel.PropertyChanged += OnViewModelPropertyChanged;|}
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

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern)
            .WithLocation(0)
            .WithArguments("Counter");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CanonicalManualPatternWithReversedInequality_ReportsInfoDiagnostic()
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
    public EventCallback<int> CounterChanged { get; set; }

    protected override void OnInitialized()
    {
        {|#0:ViewModel.PropertyChanged += OnViewModelPropertyChanged;|}
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if ((e.PropertyName == nameof(ViewModel.Counter)) && (Counter != ViewModel.Counter))
        {
            await this.CounterChanged.InvokeAsync(this.ViewModel.Counter);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern)
            .WithLocation(0)
            .WithArguments("Counter");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task OnInitializedAsyncSubscription_NoDiagnostic()
    {
        const string test = @"
using System.ComponentModel;
using System.Threading.Tasks;
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

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
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

public class TestViewModel : ViewModelBase
{
    [ViewParameter]
    public int Counter { get; set; }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task StateHasChangedOnlyHandler_NoDiagnostic()
    {
        const string test = @"
using System.ComponentModel;
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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Counter))
        {
            StateHasChanged();
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

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task MultiPropertyHandler_NoDiagnostic()
    {
        const string test = @"
using System.ComponentModel;
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
        else if (e.PropertyName == nameof(ViewModel.OtherValue))
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

    [ViewParameter]
    public int OtherValue { get; set; }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
