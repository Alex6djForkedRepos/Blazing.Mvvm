using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazing.Mvvm.Analyzers.Analyzers;

/// <summary>
/// Analyzer that validates MvvmNavLink TViewModel parameter references valid registered ViewModels.
/// Works with both C# generic syntax and Razor component attribute syntax.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MvvmNavLinkTypeSafetyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.MvvmNavLinkInvalidViewModel);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        
        context.RegisterCompilationStartAction(compilationContext =>
        {
            // Analyze GenericNameSyntax nodes to find MvvmNavLink<TViewModel> usages
            compilationContext.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var genericName = (GenericNameSyntax)syntaxContext.Node;

                // Check if this is MvvmNavLink
                if (genericName.Identifier.ValueText != "MvvmNavLink")
                    return;

                // Get semantic info
                var typeInfo = syntaxContext.SemanticModel.GetTypeInfo(genericName, syntaxContext.CancellationToken);
                
                if (typeInfo.Type is not INamedTypeSymbol namedType)
                    return;

                // Verify it's from Blazing.Mvvm.Components.Routing namespace
                var ns = namedType.OriginalDefinition.ContainingNamespace?.ToDisplayString();
                if (ns != "Blazing.Mvvm.Components.Routing")
                    return;

                // Extract TViewModel type argument
                if (namedType.TypeArguments.Length == 0)
                    return;

                var viewModelType = namedType.TypeArguments[0] as INamedTypeSymbol;
                if (viewModelType == null)
                    return;

                // Skip interfaces - they don't need [ViewModelDefinition]
                if (viewModelType.TypeKind == TypeKind.Interface)
                    return;

                // Validate the referenced symbol directly. Syntax and symbol analyzer actions can run
                // concurrently, so relying on a separately populated collection creates a race.
                if (!IsValidViewModel(viewModelType, syntaxContext.Compilation))
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.MvvmNavLinkInvalidViewModel,
                        genericName.GetLocation(),
                        viewModelType.Name);

                    syntaxContext.ReportDiagnostic(diagnostic);
                }
                
            }, SyntaxKind.GenericName);

            // Also check ObjectCreationExpressionSyntax for direct C# usage
            compilationContext.RegisterSyntaxNodeAction(syntaxContext =>
            {
                var creation = (ObjectCreationExpressionSyntax)syntaxContext.Node;
                
                // Check if the type is a GenericNameSyntax (e.g., new MvvmNavLink<TViewModel>())
                if (creation.Type is not GenericNameSyntax genericType)
                    return;

                // Check if this is MvvmNavLink
                if (genericType.Identifier.ValueText != "MvvmNavLink")
                    return;

                // Get semantic info
                var typeInfo = syntaxContext.SemanticModel.GetTypeInfo(creation, syntaxContext.CancellationToken);
                
                if (typeInfo.Type is not INamedTypeSymbol namedType)
                    return;

                // Verify it's from Blazing.Mvvm.Components.Routing namespace
                var ns = namedType.OriginalDefinition.ContainingNamespace?.ToDisplayString();
                if (ns != "Blazing.Mvvm.Components.Routing")
                    return;

                // Extract TViewModel type argument
                if (namedType.TypeArguments.Length == 0)
                    return;

                var viewModelType = namedType.TypeArguments[0] as INamedTypeSymbol;
                if (viewModelType == null)
                    return;

                // Skip interfaces - they don't need [ViewModelDefinition]
                if (viewModelType.TypeKind == TypeKind.Interface)
                    return;

                if (!IsValidViewModel(viewModelType, syntaxContext.Compilation))
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.MvvmNavLinkInvalidViewModel,
                        genericType.GetLocation(),
                        viewModelType.Name);

                    syntaxContext.ReportDiagnostic(diagnostic);
                }
                
            }, SyntaxKind.ObjectCreationExpression);
        });
    }

    private static bool IsValidViewModel(INamedTypeSymbol typeSymbol, Compilation compilation)
    {
        if (!typeSymbol.Name.EndsWith(AnalyzerConstants.Naming.ViewModelSuffix) ||
            typeSymbol.TypeKind != TypeKind.Class ||
            typeSymbol.IsAbstract ||
            !InheritsFromViewModelBase(typeSymbol, compilation))
        {
            return false;
        }

        return typeSymbol.GetAttributes().Any(attribute =>
            attribute.AttributeClass?.Name == AnalyzerConstants.AttributeNames.ViewModelDefinition ||
            attribute.AttributeClass?.Name == AnalyzerConstants.AttributeNames.ViewModelDefinition + "Attribute");
    }

    /// <summary>
    /// Checks if a type inherits from ViewModelBase (same logic as BLAZMVVM0001)
    /// </summary>
    private static bool InheritsFromViewModelBase(INamedTypeSymbol typeSymbol, Compilation compilation)
    {
        var viewModelBaseTypes = new[]
        {
            compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.ViewModelBase),
            compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.RecipientViewModelBase),
            compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.ValidatorViewModelBase)
        };

        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (viewModelBaseTypes.Any(vb => vb != null && SymbolEqualityComparer.Default.Equals(baseType, vb)))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        return false;
    }
}
