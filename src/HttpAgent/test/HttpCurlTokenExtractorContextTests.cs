// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpCurlTokenExtractorContextTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpCurlTokenExtractorContext(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var tokens = new List<string> { "curl", "-X", "POST" };
        var context = new HttpCurlTokenExtractorContext(tokens);

        Assert.Same(tokens, context.Tokens);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Equal("curl", context.CurrentToken);
        Assert.True(context.HasNext);
        Assert.False(context.IsEndOfTokens);
        Assert.Equal("-X", context.PeekNext());
    }

    [Fact]
    public void CurrentToken_Invalid_Parameters()
    {
        var tokens = new List<string> { "curl" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        context.Advance();
        var exception = Assert.Throws<InvalidOperationException>(() => context.CurrentToken);
        Assert.Equal("Token index out of range.", exception.Message);
    }

    [Fact]
    public void CurrentToken_ReturnOK()
    {
        var tokens = new List<string> { "first", "second" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        Assert.Equal("first", context.CurrentToken);
        context.Advance();
        Assert.Equal("second", context.CurrentToken);
    }

    [Fact]
    public void HasNext_ReturnOK()
    {
        var tokens = new List<string> { "a" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        Assert.False(context.HasNext);

        tokens = new List<string> { "a", "b" };
        context = new HttpCurlTokenExtractorContext(tokens);
        Assert.True(context.HasNext);
        context.Advance();
        Assert.False(context.HasNext);
    }

    [Fact]
    public void IsEndOfTokens_ReturnOK()
    {
        var tokens = new List<string> { "a" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        Assert.False(context.IsEndOfTokens);
        context.Advance();
        Assert.True(context.IsEndOfTokens);
    }

    [Fact]
    public void Advance_ReturnOK()
    {
        var tokens = new List<string> { "a", "b", "c", "d" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        Assert.Equal(0, context.CurrentIndex);

        context.Advance();
        Assert.Equal(1, context.CurrentIndex);
        Assert.Equal("b", context.CurrentToken);

        context.Advance(2);
        Assert.Equal(3, context.CurrentIndex);
        Assert.Equal("d", context.CurrentToken);

        context.Advance(5); // 超出，索引继续增加
        Assert.Equal(8, context.CurrentIndex);
        Assert.True(context.IsEndOfTokens);
    }

    [Fact]
    public void Reset_ReturnOK()
    {
        var tokens = new List<string> { "a", "b" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        context.Advance(2);
        Assert.True(context.IsEndOfTokens);

        context.Reset();
        Assert.Equal(0, context.CurrentIndex);
        Assert.Equal("a", context.CurrentToken);
    }

    [Fact]
    public void PeekNext_ReturnOK()
    {
        var tokens = new List<string> { "curl", "-X" };
        var context = new HttpCurlTokenExtractorContext(tokens);
        Assert.Equal("-X", context.PeekNext());
        context.Advance();
        Assert.Null(context.PeekNext());
    }

    [Fact]
    public void CurrentTokenMatches_ReturnOK()
    {
        var tokens = new List<string> { "curl", "-X", "POST" };
        var context = new HttpCurlTokenExtractorContext(tokens);

        Assert.True(context.CurrentTokenMatches("curl"));
        Assert.True(context.CurrentTokenMatches("CURL", "other"));
        Assert.False(context.CurrentTokenMatches("wget"));

        context.Advance();
        Assert.True(context.CurrentTokenMatches("-X", "--request"));

        context.Advance(); // 指向 "POST"
        Assert.True(context.CurrentTokenMatches("post"));

        context.Advance(); // 超出范围
        Assert.False(context.CurrentTokenMatches("anything"));
    }
}