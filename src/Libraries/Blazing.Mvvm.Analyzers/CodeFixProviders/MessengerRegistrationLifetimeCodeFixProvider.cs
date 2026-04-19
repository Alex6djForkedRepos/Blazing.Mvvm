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

namespace Blazing.Mvvm.Analyzers.CodeFixProviders;

/// <summary>
/// Code fix provider for messenger registration lifetime analyzer.
/// Adds proper unregistration or converts to OnActivated pattern.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MessengerRegistrationLifetimeCodeFixProvider))]
[Shared]
public sealed class MessengerRegistrationLifetimeCodeFixProvider : CodeFixProvider
{
    private const string TitleDispose = "Override Dispose(bool) with Unregister";
    private const string TitleOnActivated = "Use OnActivated pattern";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticDescriptors.MessengerRegistrationLeakPossible.Id);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var classDeclaration = root.FindToken(diagnosticSpan.Start)
            .Parent?
            .AncestorsAndSelf()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();

        if (classDeclaration is null)
        {
            return;
        }

        // Check if class inherits from RecipientViewModelBase
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return;
        }

        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);
        var isRecipientViewModel = classSymbol?.BaseType?.Name == "RecipientViewModelBase";

        // Offer OnActivated pattern for RecipientViewModelBase
        if (isRecipientViewModel)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: TitleOnActivated,
                    createChangedDocument: cancellationToken => UseOnActivatedPatternAsync(
                        context.Document,
                        classDeclaration,
                        cancellationToken),
                    equivalenceKey: TitleOnActivated),
                diagnostic);
        }

        // Always offer Dispose pattern
        context.RegisterCodeFix(
            CodeAction.Create(
                title: TitleDispose,
                createChangedDocument: cancellationToken => AddDisposeWithUnregisterAsync(
                    context.Document,
                    classDeclaration,
                    cancellationToken),
                equivalenceKey: TitleDispose),
            diagnostic);
    }

    /// <summary>
    /// Converts constructor registration to OnActivated pattern.
    /// </summary>
    private static async Task<Document> UseOnActivatedPatternAsync(
        Document document,
        ClassDeclarationSyntax classDeclaration,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        // Create OnActivated method
        var onActivatedMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier("OnActivated"))
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement("// Move Messenger.Register calls here from constructor")));

        var newClassDeclaration = classDeclaration.AddMembers(onActivatedMethod);
        editor.ReplaceNode(classDeclaration, newClassDeclaration);

        return editor.GetChangedDocument();
    }

    /// <summary>
    /// Adds Dispose(bool) override with Unregister calls.
    /// </summary>
    private static async Task<Document> AddDisposeWithUnregisterAsync(
        Document document,
        ClassDeclarationSyntax classDeclaration,
        CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

        var hasDisposeOverride = classDeclaration.Members
            .OfType<MethodDeclarationSyntax>()
            .Any(method =>
                method.Identifier.ValueText == "Dispose" &&
                method.Modifiers.Any(SyntaxKind.OverrideKeyword) &&
                method.ParameterList.Parameters.Count == 1 &&
                method.ParameterList.Parameters[0].Type?.ToString() == "bool");

        if (hasDisposeOverride)
        {
            return document;
        }

        var newClassDeclaration = classDeclaration;

        // Create Dispose override
        var disposeMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                SyntaxFactory.Identifier("Dispose"))
            .AddModifiers(
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("disposing"))
                    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword))))
            .WithBody(
                SyntaxFactory.Block(
                    SyntaxFactory.ParseStatement("if (disposing)\n            {\n                Messenger.UnregisterAll(this);\n            }"),
                    SyntaxFactory.ParseStatement("base.Dispose(disposing);")));

        newClassDeclaration = newClassDeclaration.AddMembers(disposeMethod);
        editor.ReplaceNode(classDeclaration, newClassDeclaration);

        return editor.GetChangedDocument();
    }
}
