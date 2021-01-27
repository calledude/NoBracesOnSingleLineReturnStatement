using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace NoBracesOnSingleLineReturnStatement
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NoBracesOnSingleLineReturnStatementAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTICID = "NoBracesOnSingleLineReturnStatement";
        private const string CATEGORY = "Formatting";

        private static readonly string _title = Resources.AnalyzerTitle;
        private static readonly string _messageFormat = Resources.AnalyzerMessageFormat;
        private static readonly string _description = Resources.AnalyzerDescription;
        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DIAGNOSTICID, _title,
                                                                                      _messageFormat, CATEGORY,
                                                                                      DiagnosticSeverity.Info,
                                                                                      isEnabledByDefault: true,
                                                                                      description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(AnalyzeSyntax);
        }

        private static void AnalyzeSyntax(SyntaxTreeAnalysisContext context)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            foreach (var blockSyntax in root.DescendantNodes().OfType<BlockSyntax>())
            {
                if (!(blockSyntax.Parent is IfStatementSyntax ifStatement))
                    continue;

                if (ifStatement.Parent is ElseClauseSyntax)
                    continue;

                if (ifStatement.Else != null)
                    continue;

                if (blockSyntax.Statements.Count == 1 && blockSyntax.Statements[0] is ReturnStatementSyntax)
                {
                    var diagnostic = Diagnostic.Create(_rule, ifStatement.GetFirstToken().GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
