using Microsoft.CodeAnalysis.Testing;

namespace Blazing.Mvvm.Analyzers.Tests;

internal static class AnalyzerTestReferenceAssemblies
{
    internal static ReferenceAssemblies Net80 { get; } = ReferenceAssemblies.Net.Net80
        .AddPackages(
        [
            // Keep framework packages at the minimum supported .NET 8 API surface.
            new PackageIdentity("Microsoft.AspNetCore.Components", "8.0.0"),
            new PackageIdentity("Microsoft.AspNetCore.Components.Web", "8.0.0"),
            new PackageIdentity("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
            new PackageIdentity("Microsoft.EntityFrameworkCore", "8.0.0"),

            // Match the CommunityToolkit.Mvvm version consumed by Blazing.Mvvm.
            new PackageIdentity("CommunityToolkit.Mvvm", "8.4.2"),
        ]);
}
