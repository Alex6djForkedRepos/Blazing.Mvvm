using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpCodeFixVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.LifecycleMethodOverrideAnalyzer,
    Blazing.Mvvm.Analyzers.CodeFixProviders.LifecycleMethodOverrideCodeFixProvider>;

namespace Blazing.Mvvm.Analyzers.Tests.CodeFixTests;

/// <summary>
/// Unit tests for <see cref="CodeFixProviders.LifecycleMethodOverrideCodeFixProvider"/>.
/// </summary>
public class LifecycleMethodOverrideCodeFixProviderTests
{
    [Fact]
    public async Task LifecycleMethod_AddsOverrideModifier()
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

        const string fixedCode = """
            using System.Threading.Tasks;
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public override async Task OnInitializedAsync()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.LifecycleMethodShouldOverride)
            .WithLocation(0)
            .WithArguments("OnInitializedAsync", "ProductViewModel");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }

    [Fact]
    public async Task VirtualLifecycleMethod_ReplacesVirtualWithOverride()
    {
        const string test = """
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public virtual bool {|#0:ShouldRender|}() => false;
            }
            """;

        const string fixedCode = """
            using Blazing.Mvvm.ComponentModel;

            public class ProductViewModel : ViewModelBase
            {
                public override bool ShouldRender() => false;
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.LifecycleMethodShouldOverride)
            .WithLocation(0)
            .WithArguments("ShouldRender", "ProductViewModel");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }
}
