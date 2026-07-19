using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Blazing.Mvvm.Analyzers.Tests.CSharpCodeFixVerifier<
    Blazing.Mvvm.Analyzers.Analyzers.RelayCommandAsyncAnalyzer,
    Blazing.Mvvm.Analyzers.CodeFixProviders.RelayCommandAsyncCodeFixProvider>;

namespace Blazing.Mvvm.Analyzers.Tests.CodeFixTests;

/// <summary>
/// Unit tests for <see cref="CodeFixProviders.RelayCommandAsyncCodeFixProvider"/>.
/// </summary>
public class RelayCommandAsyncCodeFixProviderTests
{
    [Fact]
    public async Task AsyncVoidRelayCommand_ReturnsTask()
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

        const string fixedCode = """
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

        var expected = new DiagnosticResult(DiagnosticDescriptors.RelayCommandAsyncVoid)
            .WithLocation(0)
            .WithArguments("Save");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }

    [Fact]
    public async Task AsyncVoidRelayCommand_AddsTasksUsing()
    {
        const string test = """
            using CommunityToolkit.Mvvm.Input;

            public partial class ProductViewModel
            {
                [RelayCommand]
                private async void {|#0:Save|}()
                {
                    await System.Threading.Tasks.Task.CompletedTask;
                }
            }
            """;

        const string fixedCode = """
            using CommunityToolkit.Mvvm.Input;
            using System.Threading.Tasks;

            public partial class ProductViewModel
            {
                [RelayCommand]
                private async Task Save()
                {
                    await System.Threading.Tasks.Task.CompletedTask;
                }
            }
            """;

        var expected = new DiagnosticResult(DiagnosticDescriptors.RelayCommandAsyncVoid)
            .WithLocation(0)
            .WithArguments("Save");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }

    [Fact]
    public async Task AsyncVoidRelayCommand_GlobalTasksUsing_DoesNotAddFileUsing()
    {
        const string test = """
            global using System.Threading.Tasks;
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

        const string fixedCode = """
            global using System.Threading.Tasks;
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

        var expected = new DiagnosticResult(DiagnosticDescriptors.RelayCommandAsyncVoid)
            .WithLocation(0)
            .WithArguments("Save");

        await VerifyCS.VerifyCodeFixAsync(test, fixedCode, expected);
    }
}
