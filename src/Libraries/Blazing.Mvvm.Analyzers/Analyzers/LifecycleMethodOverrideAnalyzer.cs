using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazing.Mvvm.Analyzers.Analyzers;

/// <summary>
/// Detects ViewModel lifecycle methods that hide, rather than override, their virtual base method.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LifecycleMethodOverrideAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<string> ViewModelBaseTypeNames =
    [
        "Blazing.Mvvm.ComponentModel.ViewModelBase",
        "Blazing.Mvvm.ComponentModel.RecipientViewModelBase",
        "Blazing.Mvvm.ComponentModel.ValidatorViewModelBase",
    ];

    private static readonly ImmutableHashSet<string> LifecycleMethodNames =
        [
            "OnInitialized",
            "OnInitializedAsync",
            "OnParametersSet",
            "OnParametersSetAsync",
            "OnAfterRender",
            "OnAfterRenderAsync",
            "ShouldRender",
        ];

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [DiagnosticDescriptors.LifecycleMethodShouldOverride];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(compilationContext =>
        {
            var supportedBaseTypes = ViewModelBaseTypeNames
                .Select(compilationContext.Compilation.GetTypeByMetadataName)
                .OfType<INamedTypeSymbol>()
                .ToImmutableArray();

            compilationContext.RegisterSymbolAction(
                symbolContext => AnalyzeMethod(symbolContext, supportedBaseTypes),
                SymbolKind.Method);
        });
    }

    private static void AnalyzeMethod(
        SymbolAnalysisContext context,
        ImmutableArray<INamedTypeSymbol> supportedBaseTypes)
    {
        var method = (IMethodSymbol)context.Symbol;

        if (method.MethodKind != MethodKind.Ordinary ||
            method.IsStatic ||
            method.IsOverride ||
            !LifecycleMethodNames.Contains(method.Name) ||
            !InheritsFromSupportedViewModelBase(method.ContainingType, supportedBaseTypes))
        {
            return;
        }

        var declaration = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(context.CancellationToken)
            as MethodDeclarationSyntax;

        // An explicit `new` communicates that hiding the lifecycle method is intentional.
        if (declaration is null || declaration.Modifiers.Any(SyntaxKind.NewKeyword))
        {
            return;
        }

        var baseMethod = FindMatchingVirtualBaseMethod(method);
        if (baseMethod is null)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.LifecycleMethodShouldOverride,
            declaration.Identifier.GetLocation(),
            method.Name,
            method.ContainingType.Name));
    }

    private static bool InheritsFromSupportedViewModelBase(
        INamedTypeSymbol type,
        ImmutableArray<INamedTypeSymbol> supportedBaseTypes)
    {
        for (var baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            if (supportedBaseTypes.Any(supportedBaseType =>
                    SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, supportedBaseType)))
            {
                return true;
            }
        }

        return false;
    }

    private static IMethodSymbol? FindMatchingVirtualBaseMethod(IMethodSymbol method)
    {
        for (var baseType = method.ContainingType.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            foreach (var candidate in baseType.GetMembers(method.Name).OfType<IMethodSymbol>())
            {
                if ((candidate.IsVirtual || candidate.IsAbstract || candidate.IsOverride) &&
                    !candidate.IsSealed &&
                    method.DeclaredAccessibility == candidate.DeclaredAccessibility &&
                    HasMatchingSignature(method, candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    private static bool HasMatchingSignature(IMethodSymbol method, IMethodSymbol candidate)
    {
        if (method.Arity != candidate.Arity ||
            method.Parameters.Length != candidate.Parameters.Length ||
            !SymbolEqualityComparer.Default.Equals(method.ReturnType, candidate.ReturnType))
        {
            return false;
        }

        for (var index = 0; index < method.Parameters.Length; index++)
        {
            var parameter = method.Parameters[index];
            var candidateParameter = candidate.Parameters[index];
            if (parameter.RefKind != candidateParameter.RefKind ||
                !SymbolEqualityComparer.Default.Equals(parameter.Type, candidateParameter.Type))
            {
                return false;
            }
        }

        return true;
    }
}
