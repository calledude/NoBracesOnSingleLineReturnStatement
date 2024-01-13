using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NoBracesOnSingleLineReturnStatement;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoBracesOnSingleLineReturnStatementAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId_NoBraces = "IF0002";
    public const string DiagnosticId_Braces = "IF0003";
    private const string _category = "Formatting";

    private static readonly DiagnosticDescriptor _shouldNotUseBracesRule
        = new(DiagnosticId_NoBraces,
              Resources.AnalyzerTitle_NoBraces,
              Resources.AnalyzerMessageFormat_NoBraces,
              _category,
              DiagnosticSeverity.Info,
              isEnabledByDefault: true,
              description: Resources.AnalyzerDescription_NoBraces);

    private static readonly DiagnosticDescriptor _shouldUseBracesRule
        = new(DiagnosticId_Braces,
              Resources.AnalyzerTitle_Braces,
              Resources.AnalyzerMessageFormat_Braces,
              _category,
              DiagnosticSeverity.Info,
              isEnabledByDefault: true,
              description: Resources.AnalyzerDescription_Braces);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(_shouldNotUseBracesRule, _shouldUseBracesRule);

    private static readonly HashSet<SyntaxKind> _controlFlowKinds =
    [
        SyntaxKind.ReturnStatement,
        SyntaxKind.ThrowStatement,
        SyntaxKind.ContinueStatement,
        SyntaxKind.BreakStatement,
        SyntaxKind.YieldBreakStatement,
        SyntaxKind.YieldReturnStatement,
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntax);
    }

    private static void AnalyzeSyntax(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);
        foreach (var ifStatement in root.DescendantNodes().OfType<IfStatementSyntax>())
        {
            if (ifStatement.Parent is ElseClauseSyntax || ifStatement.Else is not null)
                continue;

            if (ifStatement.Statement is not BlockSyntax blockSyntax)
            {
                if (_controlFlowKinds.Contains(ifStatement.Statement.Kind()))
                    continue;

                var shouldUseBracesDiagnostic = Diagnostic.Create(_shouldUseBracesRule, ifStatement.GetLocation());
                context.ReportDiagnostic(shouldUseBracesDiagnostic);

                continue;
            }

            if (blockSyntax.Statements.Count != 1)
                continue;

            if (!_controlFlowKinds.Contains(blockSyntax.Statements[0].Kind()))
                continue;

            var statementSpan = blockSyntax.Statements.Span;
            var lineSpan = context.Tree.GetLineSpan(statementSpan);
            if (lineSpan.EndLinePosition.Line > lineSpan.StartLinePosition.Line) // This is a multiline statement
                continue;

            var shouldNotUseBraces = Diagnostic.Create(_shouldNotUseBracesRule, ifStatement.GetLocation());
            context.ReportDiagnostic(shouldNotUseBraces);
        }
    }
}
