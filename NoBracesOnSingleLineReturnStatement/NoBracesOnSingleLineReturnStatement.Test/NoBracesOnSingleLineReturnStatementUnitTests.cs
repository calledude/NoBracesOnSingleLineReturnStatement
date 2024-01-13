using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = NoBracesOnSingleLineReturnStatement.Test.CSharpCodeFixVerifier<
    NoBracesOnSingleLineReturnStatement.NoBracesOnSingleLineReturnStatementAnalyzer,
    NoBracesOnSingleLineReturnStatement.NoBracesOnSingleLineReturnStatementCodeFixProvider>;

namespace NoBracesOnSingleLineReturnStatement.Test;

[TestClass]
public class NoBracesOnSingleLineReturnStatementUnitTest
{
    //No diagnostics expected to show up
    [TestMethod]
    public async Task TestMethod1()
    {
        const string test = """
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;
        using System.Threading.Tasks;
        using System.Diagnostics;

        namespace ConsoleApplication1
        {
            public class something
            {
                public int DoTheThing(bool f)
                {
                    if (f)
                    {
                        return 5;
                    }

                    if (f)
                    {
                        return Task.Run(() =>
                        {
                            return 5;
                        }).Result;
                    }

                    if(f)
                    {
                        return 5;
                    }

                    int i = 5;
                    if (f)
                        i = 3;

                    if (f)
                        i = 4;

                    if(!f)
                        return 5;

                    Console.WriteLine(i);
                }
            }

        }
        """;

        var diagnosticDescriptor = new DiagnosticDescriptor(
            NoBracesOnSingleLineReturnStatementAnalyzer.DiagnosticId_NoBraces,
            "Complete method not called",
            "Complete method was not called or otherwise unreachable",
            "Bug",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Complete method must be called.");

        await VerifyCS.VerifyAnalyzerAsync(
                test,
                new DiagnosticResult(diagnosticDescriptor));
    }
}
