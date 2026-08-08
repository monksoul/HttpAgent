// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpCurlTokenizerTests
{
    [Fact]
    public void Tokenize_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpCurlTokenizer.Tokenize(null!));
        Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("   \t  "));

        var exception1 =
            Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("-H \"Content-Type: application/json"));
        Assert.Equal("Unterminated quote in cURL command.", exception1.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("-d 'hello"));
        Assert.Equal("Unterminated quote in cURL command.", exception2.Message);

        var exception3 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("curl \"unclosed"));
        Assert.Equal("Unterminated quote in cURL command.", exception3.Message);

        var exception4 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("curl 'unclosed"));
        Assert.Equal("Unterminated quote in cURL command.", exception4.Message);

        var exception5 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("'unclosed"));
        Assert.Equal("Unterminated quote in cURL command.", exception5.Message);

        var exception6 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("\"unclosed"));
        Assert.Equal("Unterminated quote in cURL command.", exception6.Message);

        var exception7 = Assert.Throws<ArgumentException>(() => HttpCurlTokenizer.Tokenize("\"hello\\\""));
        Assert.Equal("Unterminated quote in cURL command.", exception7.Message);
    }

    [Fact]
    public void Tokenize_SimpleCommand_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("curl http://example.com");
        Assert.Equal(["curl", "http://example.com"], tokens);
    }

    [Fact]
    public void Tokenize_CommandWithSingleQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d 'hello world'");
        Assert.Equal(["-d", "hello world"], tokens);
    }

    [Fact]
    public void Tokenize_CommandWithDoubleQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"hello world\"");
        Assert.Equal(["-d", "hello world"], tokens);
    }

    [Fact]
    public void Tokenize_MixedQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"it's a test\"");
        Assert.Equal(["-d", "it's a test"], tokens);
    }

    [Fact]
    public void Tokenize_EscapedDoubleQuoteInDoubleQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("echo \"say \\\"hello\\\"\"");
        Assert.Equal(["echo", "say \"hello\""], tokens);
    }

    [Fact]
    public void Tokenize_EscapedSingleQuoteInSingleQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("echo 'it\\'s'");
        Assert.Equal(["echo", "it's"], tokens);
    }

    [Fact]
    public void Tokenize_EscapedBackslash_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("echo \"a\\\\b\"");
        Assert.Equal(["echo", "a\\b"], tokens);
    }

    [Fact]
    public void Tokenize_LinuxLineContinuation_ReturnOK()
    {
        const string cmd = "curl --data 'hello'\\\n 'world'";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "--data", "hello", "world"], tokens);
    }

    [Fact]
    public void Tokenize_WindowsLineContinuation_ReturnOK()
    {
        const string cmd = "curl ^\n -X POST";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "-X", "POST"], tokens);
    }

    [Fact]
    public void Tokenize_MultipleSpacesAndTabs_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("curl   -X\tPOST\thttp://example.com");
        Assert.Equal(["curl", "-X", "POST", "http://example.com"], tokens);
    }

    [Fact]
    public void Tokenize_EmptyQuotedString_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"\"");
        Assert.Equal(["-d"], tokens);
    }

    [Fact]
    public void Tokenize_TokenAtEndWithoutTrailingSpace_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"hello\"");
        Assert.Equal(["-d", "hello"], tokens);
    }

    [Fact]
    public void Tokenize_NormalEscapedChar_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("\\-X GET");
        Assert.Equal(["\\-X", "GET"], tokens);
    }

    [Fact]
    public void Tokenize_ComplexCurlCommand_ReturnOK()
    {
        const string cmd =
            "curl -k -X POST 'https://localhost:7044/HttpRemote/AddUrlForm' \\\n -H 'Content-Type: application/x-www-form-urlencoded' \\\n --data-urlencode 'id=200' \\\n --data-urlencode 'name=fu rion'";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        var expected = new List<string>
        {
            "curl",
            "-k",
            "-X",
            "POST",
            "https://localhost:7044/HttpRemote/AddUrlForm",
            "-H",
            "Content-Type: application/x-www-form-urlencoded",
            "--data-urlencode",
            "id=200",
            "--data-urlencode",
            "name=fu rion"
        };
        Assert.Equal(expected, tokens);
    }

    [Fact]
    public void Tokenize_NewlineInsideQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"line1\nline2\"");
        Assert.Equal(["-d", "line1\nline2"], tokens);
    }

    [Fact]
    public void Tokenize_CarriageReturnInsideQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"line1\rline2\"");
        Assert.Equal(["-d", "line1\rline2"], tokens);
    }

    [Fact]
    public void Tokenize_MultipleLineContinuations_ReturnOK()
    {
        const string cmd = "curl \\\n -L \\\n http://example.com";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "-L", "http://example.com"], tokens);
    }

    [Fact]
    public void Tokenize_WindowsContinuationWithCarriageReturn_ReturnOK()
    {
        const string cmd = "curl ^\r\n -L";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "-L"], tokens);
    }

    [Fact]
    public void Tokenize_SingleDashOption_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-I");
        Assert.Equal(["-I"], tokens);
    }

    [Fact]
    public void Tokenize_OnlyCurlKeyword_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("curl");
        Assert.Equal(["curl"], tokens);
    }

    [Fact]
    public void Tokenize_TabBetweenTokens_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("curl\t-X\tGET");
        Assert.Equal(["curl", "-X", "GET"], tokens);
    }

    [Fact]
    public void Tokenize_LeadingAndTrailingSpaces_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("  curl -X GET  ");
        Assert.Equal(["curl", "-X", "GET"], tokens);
    }

    [Fact]
    public void Tokenize_OnlyWhitespaceInsideQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d '   '");
        Assert.Equal(["-d", "   "], tokens);
    }

    [Fact]
    public void Tokenize_EmptySingleQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d ''");
        Assert.Equal(["-d"], tokens);
    }

    [Fact]
    public void Tokenize_OptionWithImmediateValue_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-XPOST");
        Assert.Equal(["-XPOST"], tokens);
    }

    [Fact]
    public void Tokenize_AtSignInValue_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-u user@domain.com");
        Assert.Equal(["-u", "user@domain.com"], tokens);
    }

    [Fact]
    public void Tokenize_ColonInValue_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-H \"X-Custom: value\"");
        Assert.Equal(["-H", "X-Custom: value"], tokens);
    }

    [Fact]
    public void Tokenize_EqualSignInValue_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("--data 'key=value'");
        Assert.Equal(["--data", "key=value"], tokens);
    }

    [Fact]
    public void Tokenize_MultipleShortOptionsCombined_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-vL");
        Assert.Equal(["-vL"], tokens);
    }

    [Fact]
    public void Tokenize_EscapedBackslashAtEndOfLine_ReturnOK()
    {
        const string cmd = "curl \\\\\n http://example.com";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "\\", "http://example.com"], tokens);
    }

    [Fact]
    public void Tokenize_OnlyEmptyQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("''");
        Assert.Empty(tokens);
    }

    [Fact]
    public void Tokenize_LineContinuationAtEnd_ReturnOK()
    {
        const string cmd = "curl --data \"hello\" \\\n";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "--data", "hello"], tokens);
    }

    [Fact]
    public void Tokenize_OptionWithEqualsSign_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("--data=raw");
        Assert.Equal(["--data=raw"], tokens);
    }

    [Fact]
    public void Tokenize_SpecialShellCharacters_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d 'a;b|c$d!e{}f*g?h[i]j'");
        Assert.Equal(["-d", "a;b|c$d!e{}f*g?h[i]j"], tokens);
    }

    [Fact]
    public void Tokenize_MixedSpacesAndContinuations_ReturnOK()
    {
        const string cmd = "curl   -X  \\\n \t POST  http://example.com";
        var tokens = HttpCurlTokenizer.Tokenize(cmd);
        Assert.Equal(["curl", "-X", "POST", "http://example.com"], tokens);
    }

    [Fact]
    public void Tokenize_LiteralBackslashInQuotes_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d \"a\\b\"");
        Assert.Equal(["-d", "a\\b"], tokens);
    }

    [Fact]
    public void Tokenize_EmptyQuotedValueFollowedByOption_ReturnOK()
    {
        var tokens = HttpCurlTokenizer.Tokenize("-d '' -H 'X: Y'");
        Assert.Equal(["-d", "-H", "X: Y"], tokens);
    }
}