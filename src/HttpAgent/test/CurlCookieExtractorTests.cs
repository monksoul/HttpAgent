// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlCookieExtractorTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var ctor = typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(ctor);
        return (HttpRequestBuilder)ctor.Invoke(null);
    }

    private static HttpCurlTokenExtractorContext CreateContext(params string[] tokens) => new(tokens);

    [Fact]
    public void Flags_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var flagsProperty = typeof(CurlCookieExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-b", "--cookie"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("curl", "http://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.Cookies);
    }

    [Fact]
    public void TryExtract_CookieFlagWithArgument_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-b", "session=abc123; token=xyz");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.Cookies);
        Assert.Equal(2, builder.Cookies!.Count);
        Assert.Equal("abc123", builder.Cookies["session"]);
        Assert.Equal("xyz", builder.Cookies["token"]);
    }

    [Fact]
    public void TryExtract_LongCookieFlagWithArgument_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--cookie", "name=value");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.Cookies);
        Assert.Single(builder.Cookies);
        Assert.Equal("value", builder.Cookies["name"]);
    }

    [Fact]
    public void TryExtract_CookieFlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--cookie");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.Cookies);
    }

    [Fact]
    public void TryExtract_CookieFlagWithArgumentAndExtraTokens_ReturnOK()
    {
        var extractor = new CurlCookieExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-b", "a=1; b=2", "-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("-X", context.CurrentToken);
        Assert.NotNull(builder.Cookies);
        Assert.Equal("1", builder.Cookies!["a"]);
        Assert.Equal("2", builder.Cookies["b"]);
    }
}