using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MyRoslynAnalyzer
{
    /// <summary>
    /// oneという名前のローカル変数を宣言していたら指摘する
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OneAnalyzer : DiagnosticAnalyzer
    {
        // 診断ID
        public const string DiagnosticId = "MyOne";

        // 診断を表すオブジェクト
        private static readonly DiagnosticDescriptor OneRule =
            new DiagnosticDescriptor(DiagnosticId, "one", "oneを使ってしまっている", "", DiagnosticSeverity.Warning, isEnabledByDefault: true);

        // このAnalyzerがサポートする診断
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(OneRule);

        public override void Initialize(AnalysisContext context)
        {
            // 自動生成コードは診断しない
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            // 並列実行するか
            context.EnableConcurrentExecution();
            // 種類がLocalDeclarationStatementであるSyntaxNodeそれぞれに対して呼ばれるActionを登録する
            // 他にも似たようなのが色々ある
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LocalDeclarationStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            // 対象をLocalDeclarationStatementに絞っているのでキャストは必ず成功する
            var localDeclaration = (LocalDeclarationStatementSyntax) context.Node;
            // 名前がoneである変数宣言を探す
            var oneVariable = localDeclaration.Declaration.Variables.FirstOrDefault(x => x.Identifier.Text == "one");
            if (oneVariable != null)
            {
                // oneがあったら診断を報告する
                context.ReportDiagnostic(Diagnostic.Create(OneRule, oneVariable.GetLocation()));
            }
        }
    }
}
