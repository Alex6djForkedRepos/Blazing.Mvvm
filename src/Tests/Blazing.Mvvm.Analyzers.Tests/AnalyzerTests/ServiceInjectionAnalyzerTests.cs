using Blazing.Mvvm.Analyzers.Analyzers;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpAnalyzerVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.ServiceInjectionAnalyzer>;

namespace Blazing.Mvvm.Analyzers.Tests.AnalyzerTests;

/// <summary>
/// Unit tests for <see cref="ServiceInjectionAnalyzer"/>
/// </summary>
public class ServiceInjectionAnalyzerTests
{
    [Fact]
    public async Task EmptyCode_NoDiagnostic()
    {
        const string test = "";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ConstructorInjectedInterface_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(IUnregisteredService service)
        {
        }
    }

    public interface IUnregisteredService { }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task CommonFrameworkServices_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(ILogger<TestViewModel> logger)
        {
            // ILogger is always registered
        }
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task KnownBlazorServices_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(NavigationManager nav, HttpClient http)
        {
            // Framework services
        }
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task InjectPropertyInViewModel_ReportsDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        {|#0:[Inject]
        public IMyCustomService MyCustomService { get; set; } = null!;|}
    }

    public interface IMyCustomService { }
}";

        var expected = new DiagnosticResult(DiagnosticDescriptors.ServiceNotRegistered)
            .WithLocation(0)
            .WithArguments("MyCustomService");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task PrimitiveTypeParameter_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(string title, int maxItems)
        {
            // Primitive parameters are not services
        }
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task ParameterlessConstructor_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel()
        {
        }
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task MultipleInjectPropertiesInViewModel_ReportMultipleDiagnostics()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        {|#0:[Inject]
        public IFirstService FirstService { get; set; } = null!;|}

        {|#1:[Inject]
        public ISecondService SecondService { get; set; } = null!;|}
    }

    public interface IFirstService { }
    public interface ISecondService { }
}";

        var expected1 = new DiagnosticResult(DiagnosticDescriptors.ServiceNotRegistered)
            .WithLocation(0)
            .WithArguments("FirstService");

        var expected2 = new DiagnosticResult(DiagnosticDescriptors.ServiceNotRegistered)
            .WithLocation(1)
            .WithArguments("SecondService");

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [Fact]
    public async Task ConstructorInjectedConcreteClass_NoDiagnostic()
    {
        const string test = @"
using Blazing.Mvvm.ComponentModel;

namespace TestNamespace
{
    [ViewModelDefinition]
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(MyConcreteService service)
        {
        }
    }

    public class MyConcreteService { }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
