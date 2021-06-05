using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace MyRoslynAnalyzer
{
    /// <summary>
    /// oneという名前のローカル変数をtwoにリネームする
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class OneFixProvider : CodeFixProvider
    {
        // 修正対象となる診断ID
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(OneAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // SyntaxRoot取得
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            // 診断取得
            var diagnostic = context.Diagnostics.First();
            // 診断の場所
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            // 診断された変数宣言取得
            var declaratorSyntax = root.FindToken(diagnosticSpan.Start).Parent
                .AncestorsAndSelf()
                .OfType<VariableDeclaratorSyntax>()
                .First();
            // コードに対して何をするかというのを生成
            var action = CodeAction.Create("oneをtwoに変える", ct => ChangeOneToTwoAsync(context.Document, declaratorSyntax, ct));
            // コード修正登録
            context.RegisterCodeFix(action, diagnostic);
        }

        // oneをtwoに変える
        private static async Task<Solution> ChangeOneToTwoAsync(Document document, VariableDeclaratorSyntax declaratorSyntax, CancellationToken ct)
        {
            // SemanticModel（意味解析の結果）取得
            var semanticModel = await document.GetSemanticModelAsync(ct);
            // Symbol取得
            var originalSymbol = semanticModel.GetDeclaredSymbol(declaratorSyntax);
            // Solution取得
            var solution = document.Project.Solution;
            // リネーム処理し、結果のSolutionを返す
            return await Renamer.RenameSymbolAsync(solution, originalSymbol, "two", solution.Workspace.Options, ct);
        }
    }
}
