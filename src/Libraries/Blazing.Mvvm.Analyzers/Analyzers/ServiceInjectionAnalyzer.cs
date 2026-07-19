using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blazing.Mvvm.Analyzers.Analyzers;

/// <summary>
/// Analyzer that detects [Inject] attribute usage in ViewModels and recommends constructor injection instead.
/// In Blazor MVVM, ViewModels should use constructor injection, not property injection with [Inject].
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ServiceInjectionAnalyzer : DiagnosticAnalyzer
{
    private const string InjectAttributeName = "InjectAttribute";
    private const string InjectAttributeShortName = "Inject";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.ServiceNotRegistered);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclaration = (PropertyDeclarationSyntax)context.Node;
        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
        
        if (propertySymbol == null)
        {
            return;
        }

        var containingType = propertySymbol.ContainingType;
        
        // Only analyze properties in ViewModels
        if (!IsViewModel(containingType))
        {
            return;
        }

        // Check if property has [Inject] attribute
        if (!HasInjectAttribute(propertySymbol))
        {
            return;
        }

        // Report diagnostic for [Inject] usage in ViewModel
        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptors.ServiceNotRegistered,
            propertyDeclaration.GetLocation(),
            propertySymbol.Name);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsViewModel(INamedTypeSymbol typeSymbol)
    {
        // Check if type name ends with "ViewModel"
        if (typeSymbol.Name.EndsWith(AnalyzerConstants.Naming.ViewModelSuffix, StringComparison.Ordinal))
        {
            return true;
        }

        // Check if type inherits from ViewModelBase, RecipientViewModelBase, or ValidatorViewModelBase
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            var fullTypeName = baseType.ToDisplayString();
            if (fullTypeName == AnalyzerConstants.TypeNames.ViewModelBase ||
                fullTypeName == AnalyzerConstants.TypeNames.RecipientViewModelBase ||
                fullTypeName == AnalyzerConstants.TypeNames.ValidatorViewModelBase)
            {
                return true;
            }
            baseType = baseType.BaseType;
        }

        return false;
    }

    private static bool HasInjectAttribute(IPropertySymbol propertySymbol)
    {
        foreach (var attribute in propertySymbol.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass == null)
            {
                continue;
            }

            var attributeName = attributeClass.Name;
            
            // Check for [Inject] or [InjectAttribute]
            if (attributeName == InjectAttributeShortName || 
                attributeName == InjectAttributeName)
            {
                // Verify it's from Microsoft.AspNetCore.Components namespace
                var namespaceName = attributeClass.ContainingNamespace?.ToDisplayString();
                if (namespaceName == "Microsoft.AspNetCore.Components")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
