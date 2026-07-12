using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazing.Mvvm.Analyzers.Analyzers;

/// <summary>
/// Detects asynchronous RelayCommand methods that return void.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RelayCommandAsyncAnalyzer : DiagnosticAnalyzer
{
    private const string RelayCommandAttributeName = "CommunityToolkit.Mvvm.Input.RelayCommandAttribute";

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [DiagnosticDescriptors.RelayCommandAsyncVoid];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;
        if (method.MethodKind != MethodKind.Ordinary ||
            !method.IsAsync ||
            !method.ReturnsVoid ||
            !method.GetAttributes().Any(IsRelayCommandAttribute))
        {
            return;
        }

        var declaration = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(context.CancellationToken)
            as MethodDeclarationSyntax;
        var location = declaration?.Identifier.GetLocation() ?? method.Locations.FirstOrDefault();
        if (location is null)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.RelayCommandAsyncVoid,
            location,
            method.Name));
    }

    private static bool IsRelayCommandAttribute(AttributeData attribute) =>
        attribute.AttributeClass?.ToDisplayString() == RelayCommandAttributeName;
}
