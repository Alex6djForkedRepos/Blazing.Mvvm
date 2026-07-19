using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Blazing.Mvvm.Analyzers.CodeFixProviders;

/// <summary>
/// Changes an asynchronous RelayCommand method's return type from void to Task.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RelayCommandAsyncCodeFixProvider))]
[Shared]
public sealed class RelayCommandAsyncCodeFixProvider : CodeFixProvider
{
    private const string Title = "Return Task";

    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds =>
        [DiagnosticDescriptors.RelayCommandAsyncVoid.Id];

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

        context.RegisterCodeFix(
            CodeAction.Create(
                Title,
                cancellationToken => ReturnTaskAsync(context.Document, method, cancellationToken),
                Title),
            diagnostic);
    }

    private static async Task<Document> ReturnTaskAsync(
        Document document,
        MethodDeclarationSyntax method,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
        {
            return document;
        }

        var taskType = SyntaxFactory.IdentifierName("Task").WithTriviaFrom(method.ReturnType);
        var updatedMethod = method.WithReturnType(taskType).WithAdditionalAnnotations(Formatter.Annotation);
        var updatedRoot = compilationUnit.ReplaceNode(method, updatedMethod);

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        var taskSymbol = semanticModel?.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
        var taskIsInScope = taskSymbol is not null && semanticModel!
            .LookupNamespacesAndTypes(method.ReturnType.SpanStart, name: "Task")
            .Any(symbol => SymbolEqualityComparer.Default.Equals(symbol, taskSymbol));

        if (!taskIsInScope)
        {
            var lineEnding = compilationUnit.ToFullString().IndexOf("\r\n", StringComparison.Ordinal) >= 0
                ? "\r\n"
                : "\n";
            var taskUsing = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("System.Threading.Tasks"))
                .WithTrailingTrivia(SyntaxFactory.EndOfLine(lineEnding))
                .WithAdditionalAnnotations(Formatter.Annotation);
            updatedRoot = updatedRoot.AddUsings(taskUsing);
        }

        return document.WithSyntaxRoot(updatedRoot);
    }
}
