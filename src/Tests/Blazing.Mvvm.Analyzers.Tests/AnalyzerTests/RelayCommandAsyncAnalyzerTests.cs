using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpAnalyzerVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.RelayCommandAsyncAnalyzer>;

namespace Blazing.Mvvm.Analyzers.Tests.AnalyzerTests;

/// <summary>
/// Unit tests for <see cref="Analyzers.RelayCommandAsyncAnalyzer"/>.
/// </summary>
public class RelayCommandAsyncAnalyzerTests
{
    [Fact]
    public async Task AsyncVoidRelayCommand_ReportsDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            public partial class ProductViewModel
            {
                [RelayCommand]
                private async void {|#0:Save|}()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.RelayCommandAsyncVoid)
            .WithLocation(0)
            .WithArguments("Save");

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task AsyncTaskRelayCommand_NoDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;
            using CommunityToolkit.Mvvm.Input;

            public partial class ProductViewModel
            {
                [RelayCommand]
                private async Task Save()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task SynchronousRelayCommand_NoDiagnostic()
    {
        const string test = """
            using CommunityToolkit.Mvvm.Input;

            public partial class ProductViewModel
            {
                [RelayCommand]
                private void Save()
                {
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task AsyncVoidWithoutRelayCommand_NoDiagnostic()
    {
        const string test = """
            using System.Threading.Tasks;

            public class ProductViewModel
            {
                private async void Save()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task LookalikeRelayCommandAttribute_NoDiagnostic()
    {
        const string test = """
            using System;
            using System.Threading.Tasks;

            public sealed class RelayCommandAttribute : Attribute
            {
            }

            public class ProductViewModel
            {
                [RelayCommand]
                private async void Save()
                {
                    await Task.CompletedTask;
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
