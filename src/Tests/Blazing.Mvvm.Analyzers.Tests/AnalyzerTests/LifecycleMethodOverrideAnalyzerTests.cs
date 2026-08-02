using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpAnalyzerVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.LifecycleMethodOverrideAnalyzer>;

namespace Blazing.Mvvm.Analyzers.Tests.AnalyzerTests;

/// <summary>
/// Unit tests for <see cref="Analyzers.LifecycleMethodOverrideAnalyzer"/>.
/// </summary>
public class LifecycleMethodOverrideAnalyzerTests
{
    [Fact]
    public async Task LifecycleMethodWithoutOverride_ReportsDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public async Task {|#0:OnInitializedAsync|}()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.LifecycleMethodShouldOverride)
            .WithLocation(0)
            .WithArguments("OnInitializedAsync", "ProductViewModel");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task LifecycleMethodWithOverride_NoDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public override Task OnInitializedAsync() => Task.CompletedTask;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task LifecycleMethodWithDifferentAccessibility_ReportsDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                protected Task {|#0:OnInitializedAsync|}() => Task.CompletedTask;
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.LifecycleMethodShouldOverride)
            .WithLocation(0)
            .WithArguments("OnInitializedAsync", "ProductViewModel");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task LifecycleMethodWithExplicitNew_NoDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public new Task OnInitializedAsync() => Task.CompletedTask;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task MethodOnNonViewModel_NoDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;

            public class ProductService
            {
                public Task OnInitializedAsync() => Task.CompletedTask;
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task LifecycleMethodWithDifferentSignature_NoDiagnostic()
    {
        const string test = """
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public void OnAfterRender(string value)
                {
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
