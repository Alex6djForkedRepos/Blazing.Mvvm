using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace Blazing.Mvvm.Analyzers.CodeFixProviders;

/// <summary>
/// Code fixes for EventCallback automatic two-way binding diagnostics.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventCallbackTwoWayBindingCodeFixProvider))]
[Shared]
public sealed class EventCallbackTwoWayBindingCodeFixProvider : CodeFixProvider
{
    private const string ScenarioPropertyName = "Scenario";
    private const string PropertyNameProperty = "PropertyName";
    private const string CallbackNameProperty = "CallbackName";
    private const string PropertyTypeProperty = "PropertyType";
    private const string HandlerNameProperty = "HandlerName";

    private const string ManualPatternScenario = "ManualPattern";
    private const string MissingCallbackScenario = "MissingCallback";
    private const string TypeMismatchScenario = "TypeMismatch";

    private const string RemoveManualPatternTitle = "Remove manual PropertyChanged binding";
    private const string AddEventCallbackTitle = "Add EventCallback two-way binding parameter";
    private const string FixEventCallbackTypeTitle = "Fix EventCallback type";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern.Id,
            DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback.Id,
            DiagnosticDescriptors.EventCallbackTwoWayBindingTypeMismatch.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (!SupportsCodeFixes(context.Document))
        {
            return;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        if (!diagnostic.Properties.TryGetValue(ScenarioPropertyName, out var scenario) || string.IsNullOrWhiteSpace(scenario))
        {
            return;
        }

        switch (scenario)
        {
            case ManualPatternScenario:
                context.RegisterCodeFix(
                    CodeAction.Create(
                        RemoveManualPatternTitle,
                        cancellationToken => RemoveManualPatternAsync(context.Document, diagnostic, root, cancellationToken),
                        RemoveManualPatternTitle),
                    diagnostic);
                break;

            case MissingCallbackScenario when
                diagnostic.Properties.TryGetValue(PropertyNameProperty, out var propertyName) &&
                diagnostic.Properties.TryGetValue(CallbackNameProperty, out var callbackName) &&
                diagnostic.Properties.TryGetValue(PropertyTypeProperty, out var propertyType):
                context.RegisterCodeFix(
                    CodeAction.Create(
                        AddEventCallbackTitle,
                        cancellationToken => AddEventCallbackAsync(context.Document, root, diagnostic.Location.SourceSpan, callbackName!, propertyType!, cancellationToken),
                        AddEventCallbackTitle),
                    diagnostic);
                break;

            case TypeMismatchScenario when
                diagnostic.Properties.TryGetValue(PropertyTypeProperty, out var requiredType):
                context.RegisterCodeFix(
                    CodeAction.Create(
                        FixEventCallbackTypeTitle,
                        cancellationToken => FixEventCallbackTypeAsync(context.Document, root, diagnostic.Location.SourceSpan, requiredType!, cancellationToken),
                        FixEventCallbackTypeTitle),
                    diagnostic);
                break;
        }
    }

    private static bool SupportsCodeFixes(Document document)
    {
        if (string.IsNullOrEmpty(document.FilePath))
        {
            return true;
        }

        var filePath = document.FilePath!;
        if (filePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase) ||
            filePath.EndsWith(".razor.g.cs", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return filePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<Document> AddEventCallbackAsync(
        Document document,
        SyntaxNode root,
        TextSpan diagnosticSpan,
        string callbackName,
        string propertyType,
        CancellationToken cancellationToken)
    {
        var targetProperty = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault();

        if (targetProperty is null)
        {
            return document;
        }

        var sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
        var lineEnding = sourceText.ToString().IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        var indentation = GetIndentation(targetProperty);
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        var callbackPropertyText = $"[Parameter]{lineEnding}{indentation}public EventCallback<{propertyType}> {callbackName} {{ get; set; }}";
        var callbackProperty = (PropertyDeclarationSyntax)SyntaxFactory.ParseMemberDeclaration(callbackPropertyText)!;
        callbackProperty = callbackProperty
            .WithLeadingTrivia(targetProperty.GetLeadingTrivia())
            .WithTrailingTrivia(targetProperty.GetTrailingTrivia());

        editor.InsertAfter(targetProperty, callbackProperty);

        AddUsingIfMissing(editor, root as CompilationUnitSyntax, "Microsoft.AspNetCore.Components");
        return editor.GetChangedDocument();
    }

    private static async Task<Document> FixEventCallbackTypeAsync(
        Document document,
        SyntaxNode root,
        TextSpan diagnosticSpan,
        string requiredType,
        CancellationToken cancellationToken)
    {
        var callbackProperty = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault();

        if (callbackProperty is null)
        {
            return document;
        }

        var newType = SyntaxFactory.GenericName("EventCallback")
            .WithTypeArgumentList(
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.ParseTypeName(requiredType))));

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(callbackProperty.Type, newType);
        AddUsingIfMissing(editor, root as CompilationUnitSyntax, "Microsoft.AspNetCore.Components");
        return editor.GetChangedDocument();
    }

    private static async Task<Document> RemoveManualPatternAsync(
        Document document,
        Diagnostic diagnostic,
        SyntaxNode root,
        CancellationToken cancellationToken)
    {
        if (!diagnostic.Properties.TryGetValue(HandlerNameProperty, out var handlerName) || string.IsNullOrWhiteSpace(handlerName))
        {
            return document;
        }

        var subscriptionStatement = root.FindToken(diagnostic.Location.SourceSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<ExpressionStatementSyntax>()
            .FirstOrDefault();

        if (subscriptionStatement is null)
        {
            return document;
        }

        var onInitializedMethod = subscriptionStatement.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        var containingClass = subscriptionStatement.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (onInitializedMethod is null || containingClass is null)
        {
            return document;
        }

        var handlerMethod = containingClass.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == handlerName);

        var disposeMethod = containingClass.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method =>
                method.Identifier.ValueText == AnalyzerConstants.MethodNames.Dispose &&
                method.ParameterList.Parameters.Count == 1 &&
                method.ParameterList.Parameters[0].Type is PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.BoolKeyword });

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.RemoveNode(onInitializedMethod, SyntaxRemoveOptions.KeepNoTrivia);

        if (handlerMethod is not null)
        {
            editor.RemoveNode(handlerMethod, SyntaxRemoveOptions.KeepNoTrivia);
        }

        if (disposeMethod is not null)
        {
            editor.RemoveNode(disposeMethod, SyntaxRemoveOptions.KeepNoTrivia);
        }

        var changedDocument = editor.GetChangedDocument();
        return changedDocument;
    }

    private static void AddUsingIfMissing(DocumentEditor editor, CompilationUnitSyntax? compilationUnit, string namespaceName)
    {
        if (compilationUnit is null || compilationUnit.Usings.Any(usingNode => usingNode.Name?.ToString() == namespaceName))
        {
            return;
        }

        editor.InsertBefore(
            compilationUnit.Members.FirstOrDefault() ?? (SyntaxNode)compilationUnit,
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName)));
    }

    private static string GetIndentation(SyntaxNode node)
    {
        var leadingTrivia = node.GetLeadingTrivia().ToFullString();
        var lastLineBreakIndex = Math.Max(leadingTrivia.LastIndexOf('\n'), leadingTrivia.LastIndexOf('\r'));
        return lastLineBreakIndex >= 0
            ? leadingTrivia.Substring(lastLineBreakIndex + 1)
            : leadingTrivia;
    }
}
