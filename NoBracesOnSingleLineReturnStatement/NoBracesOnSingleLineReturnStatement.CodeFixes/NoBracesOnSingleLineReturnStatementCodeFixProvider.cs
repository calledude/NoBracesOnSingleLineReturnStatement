using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace NoBracesOnSingleLineReturnStatement
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NoBracesOnSingleLineReturnStatementCodeFixProvider)), Shared]
    public class NoBracesOnSingleLineReturnStatementCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(NoBracesOnSingleLineReturnStatementAnalyzer.DIAGNOSTICID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics[0];

            context.RegisterCodeFix(
                CodeAction.Create(
                    CodeFixResources.CodeFixTitle,
                    _ => AddBracesAsync(context.Document, diagnostic.Location, root),
                    CodeFixResources.CodeFixTitle),
                diagnostic);
        }

        private Task<Document> AddBracesAsync(Document document, Location location, SyntaxNode root)
        {
            var block = root
                .FindNode(location.SourceSpan)
                .ChildNodes()
                .OfType<BlockSyntax>()
                .Single();

            var openBraceToken = SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken);
            var closeBraceToken = SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken);

            var newBlock = SyntaxFactory.Block(openBraceToken, block.Statements, closeBraceToken);
            var newRoot = root.ReplaceNode(block, newBlock);

            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }
    }
}
