using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Blazing.Mvvm.Analyzers.Tests;

/// <summary>
/// Base class for code fix provider test helpers
/// </summary>
public static class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    /// <summary>
    /// Creates a new code fix test instance
    /// </summary>
    public static CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier> CreateTest()
    {
        return new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages(ImmutableArray.Create(
                    new PackageIdentity("Microsoft.AspNetCore.Components", "8.0.0"),
                    new PackageIdentity("Microsoft.AspNetCore.Components.Web", "8.0.0"),
                    new PackageIdentity("CommunityToolkit.Mvvm", "8.3.2"),
                    new PackageIdentity("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                    new PackageIdentity("Microsoft.EntityFrameworkCore", "8.0.0")
                )),
            TestState =
            {
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(ResolveFrameworkAssembly("Microsoft.AspNetCore.App.Ref", "net8.0", "Microsoft.AspNetCore.Components.dll")),
                    MetadataReference.CreateFromFile(ResolveLatestPackageAssembly("microsoft.entityframeworkcore", "lib", "net8.0", "Microsoft.EntityFrameworkCore.dll"))
                },
                // Add Blazing.Mvvm type stubs to every test
                Sources = { TestCode.BlazingMvvmStubs }
            },
            FixedState =
            {
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(ResolveFrameworkAssembly("Microsoft.AspNetCore.App.Ref", "net8.0", "Microsoft.AspNetCore.Components.dll")),
                    MetadataReference.CreateFromFile(ResolveLatestPackageAssembly("microsoft.entityframeworkcore", "lib", "net8.0", "Microsoft.EntityFrameworkCore.dll"))
                },
                // Add Blazing.Mvvm type stubs to fixed code as well
                Sources = { TestCode.BlazingMvvmStubs }
            }
        };
    }

    /// <summary>
    /// Verifies code fix diagnostics and fix
    /// </summary>
    public static Task VerifyCodeFixAsync(string source, string fixedSource, params DiagnosticResult[] expected)
    {
        var test = CreateTest();
        test.TestCode = source;
        test.FixedCode = fixedSource;
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync();
    }

    /// <summary>
    /// Verifies code fix diagnostics only
    /// </summary>
    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = CreateTest();
        test.TestCode = source;
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync();
    }

    private static string ResolveFrameworkAssembly(string packName, string targetFramework, string assemblyName)
    {
        var packRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "packs", packName);
        var latestPack = Directory.GetDirectories(packRoot)
            .OrderByDescending(static directory => directory, StringComparer.OrdinalIgnoreCase)
            .First(directory => File.Exists(Path.Combine(directory, "ref", targetFramework, assemblyName)));

        return Path.Combine(latestPack, "ref", targetFramework, assemblyName);
    }

    private static string ResolveLatestPackageAssembly(string packageId, params string[] relativePath)
    {
        var packageRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages", packageId);
        var latestPackage = Directory.GetDirectories(packageRoot)
            .OrderByDescending(static directory => directory, StringComparer.OrdinalIgnoreCase)
            .First(directory => File.Exists(Path.Combine(new[] { directory }.Concat(relativePath).ToArray())));

        return Path.Combine(new[] { latestPackage }.Concat(relativePath).ToArray());
    }
}
