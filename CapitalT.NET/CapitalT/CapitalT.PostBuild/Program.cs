using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapitalT.PostBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            var workspace = Workspace.LoadStandAloneProject("C:/D/Repos/CapitalT/CapitalT.NET/CapitalT/Examples/CapitalT.Examples.Mvc5Example/CapitalT.Examples.Mvc5Example.csproj");
            var project = workspace.CurrentSolution.Projects.FirstOrDefault();
            var compilation = project.GetCompilation();

            var documents = project.Documents.Where(doc => doc.SourceCodeKind == Roslyn.Compilers.SourceCodeKind.Regular).Where(doc => doc.Name.Contains("HomeController")).ToList();

            foreach (var ast in compilation.SyntaxTrees)
            {
                if (!ast.FilePath.Contains("HomeController"))
                {
                    continue;
                }
                var model = compilation.GetSemanticModel(ast);

                var localizerDeclarations = ast.GetRoot().DescendantNodesAndSelf().Where(node => {
                    return node is PropertyDeclarationSyntax;
                })
                .Select(node => model.GetDeclaredSymbol(node))
                .ToList();
                
                var allSymbols = ast.GetRoot().DescendantNodesAndSelf().Select(n => model.GetSymbolInfo(n)).Where(s => s.CandidateSymbols.Count > 0).ToList();
                var expressions = ast.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                    .Where(s => {
                        var expression = s.Expression as IdentifierNameSyntax;
                        if (expression != null)
                        {
                            return expression.Identifier.ValueText == "T";
                        }
                        return false;
                    }).ToList();

                var symbols = expressions.Select(s => model.GetTypeInfo(s)).ToList();

                Debugger.Break();
            }

            Debugger.Break();
        }
    }
}
