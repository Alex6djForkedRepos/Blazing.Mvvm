using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Blazing.Mvvm.Analyzers.Analyzers;

namespace Blazing.Mvvm.Analyzers.CodeFixProviders;

/// <summary>
/// Adds the override modifier to a ViewModel lifecycle method.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LifecycleMethodOverrideCodeFixProvider))]
[Shared]
public sealed class LifecycleMethodOverrideCodeFixProvider : CodeFixProvider
{
    private const string Title = "Add override modifier";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        [DiagnosticDescriptors.LifecycleMethodShouldOverride.Id];

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var method = root?.FindToken(diagnostic.Location.SourceSpan.Start)
            .Parent?.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (method is null)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        var methodSymbol = semanticModel?.GetDeclaredSymbol(method, context.CancellationToken);
        if (methodSymbol is null)
        {
            return;
        }

        var baseMethod = LifecycleMethodOverrideAnalyzer.FindMatchingVirtualBaseMethod(methodSymbol);
        if (baseMethod is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                cancellationToken => AddOverrideAsync(
                    context.Document,
                    method,
                    methodSymbol.DeclaredAccessibility,
                    baseMethod.DeclaredAccessibility,
                    cancellationToken),
                Title),
            diagnostic);
    }

    private static async Task<Document> AddOverrideAsync(
        Document document,
        MethodDeclarationSyntax method,
        Accessibility methodAccessibility,
        Accessibility baseAccessibility,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var methodWithAccessibility = methodAccessibility == baseAccessibility
            ? method
            : (MethodDeclarationSyntax)SyntaxGenerator.GetGenerator(document)
                .WithAccessibility(method, baseAccessibility);

        var modifiers = new SyntaxTokenList(methodWithAccessibility.Modifiers.Where(token =>
            !token.IsKind(SyntaxKind.VirtualKeyword)));

        var insertionIndex = 0;
        while (insertionIndex < modifiers.Count && IsAccessibilityModifier(modifiers[insertionIndex]))
        {
            insertionIndex++;
        }
        modifiers = modifiers.Insert(
            insertionIndex,
            SyntaxFactory.Token(SyntaxKind.OverrideKeyword).WithTrailingTrivia(SyntaxFactory.Space));

        var updatedMethod = methodWithAccessibility
            .WithModifiers(modifiers)
            .WithAdditionalAnnotations(Formatter.Annotation);
        return document.WithSyntaxRoot(root.ReplaceNode(method, updatedMethod));
    }

    private static bool IsAccessibilityModifier(SyntaxToken token) =>
        token.IsKind(SyntaxKind.PublicKeyword) ||
        token.IsKind(SyntaxKind.ProtectedKeyword) ||
        token.IsKind(SyntaxKind.InternalKeyword) ||
        token.IsKind(SyntaxKind.PrivateKeyword);
}
