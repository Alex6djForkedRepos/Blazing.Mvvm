using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Blazing.Mvvm.Analyzers.Tests;

/// <summary>
/// Base class for analyzer test helpers
/// </summary>
public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// Creates a new analyzer test instance
    /// </summary>
    public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateTest()
    {
        return new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
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
                // Add Blazing.Mvvm type stubs to every test
                Sources = { TestCode.BlazingMvvmStubs }
            }
        };
    }

    /// <summary>
    /// Verifies analyzer diagnostics
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
        var packRoot = Path.Combine(GetDotNetPacksRoot(), packName);
        if (!Directory.Exists(packRoot))
            throw new InvalidOperationException(
                $"dotnet packs directory not found: '{packRoot}'. " +
                $"Install the matching .NET SDK or set the DOTNET_ROOT environment variable.");

        var packDirs = Directory.GetDirectories(packRoot)
            .OrderByDescending(static directory => directory, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        // Try the requested TFM first
        var exactMatch = packDirs.FirstOrDefault(d => File.Exists(Path.Combine(d, "ref", targetFramework, assemblyName)));
        if (exactMatch is not null)
            return Path.Combine(exactMatch, "ref", targetFramework, assemblyName);

        // Fallback: find the assembly in any available TFM (e.g. only .NET 10 SDK installed)
        foreach (var packDir in packDirs)
        {
            var refRoot = Path.Combine(packDir, "ref");
            if (!Directory.Exists(refRoot))
                continue;

            var fallback = Directory.GetDirectories(refRoot)
                .OrderByDescending(static d => d, StringComparer.OrdinalIgnoreCase)
                .Select(tfmDir => Path.Combine(tfmDir, assemblyName))
                .FirstOrDefault(File.Exists);

            if (fallback is not null)
                return fallback;
        }

        throw new InvalidOperationException($"Could not find '{assemblyName}' for '{targetFramework}' in '{packRoot}'");
    }

    private static string GetDotNetPacksRoot()
    {
        // DOTNET_ROOT is set by actions/setup-dotnet and most CI/Docker environments
        var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(dotnetRoot))
            return Path.Combine(dotnetRoot, "packs");

        // Windows: C:\Program Files\dotnet\packs
        if (Path.DirectorySeparatorChar == '\\')
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "packs");

        // macOS official installer and Homebrew location
        if (Directory.Exists("/usr/local/share/dotnet/packs"))
            return "/usr/local/share/dotnet/packs";

        // Linux default install location
        return "/usr/share/dotnet/packs";
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
