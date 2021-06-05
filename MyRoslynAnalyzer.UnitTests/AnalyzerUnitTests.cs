using System.Threading.Tasks;
using Xunit;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<MyRoslynAnalyzer.OneAnalyzer>;
using FixVerify = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<MyRoslynAnalyzer.OneAnalyzer, MyRoslynAnalyzer.OneFixProvider>;

namespace MyRoslynAnalyzer.UnitTests
{
    public class AnalyzerUnitTests
    {
        /// <summary>
        /// Analyzerのテスト
        /// </summary>
        [Fact]
        public async Task OneAnalyzeTest()
        {
            // テスト用コード
            var testCode = @"
class C
{
    public void M()
    {
        int [|one = 10|];
    }
}";
            await Verify.VerifyAnalyzerAsync(testCode);
        }

        /// <summary>
        /// 修正のテスト
        /// </summary>
        [Fact]
        public async Task OneFixTest()
        {
            // テスト用コード
            var testCode = @"
class C
{
    public void M()
    {
        int [|one = 10|];
        int hoge = one + 4;
    }
}";
            // 修正された結果のコード
            var fixedCode = @"
class C
{
    public void M()
    {
        int two = 10;
        int hoge = two + 4;
    }
}";
            await FixVerify.VerifyCodeFixAsync(testCode, fixedCode);
        }
    }
}
