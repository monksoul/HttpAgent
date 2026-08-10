// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlUrlExtractorTests
{
    private static HttpRequestBuilder CreateBuilder()
    {
        var ctor = typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null, Type.EmptyTypes, null);
        Assert.NotNull(ctor);
        return (HttpRequestBuilder)ctor.Invoke(null);
    }

    private static HttpCurlParsingContext CreateContext(params string[] tokens) => new(tokens);

    [Fact]
    public void Order_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        Assert.Equal(100, extractor.Order);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.RequestUri);
    }

    [Fact]
    public void TryExtract_ExplicitUrlWithArgument_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--url", "https://example.com/api");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("https://example.com/api", builder.RequestUri.OriginalString);
    }

    [Fact]
    public void TryExtract_ExplicitUrlWithoutArgument_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--url");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.RequestUri);
    }

    [Fact]
    public void TryExtract_ImplicitHttpUrl_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("http://example.com/path");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("http://example.com/path", builder.RequestUri.OriginalString);
    }

    [Fact]
    public void TryExtract_ImplicitHttpsUrl_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("https://secure.example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("https://secure.example.com", builder.RequestUri.OriginalString);
    }

    [Fact]
    public void TryExtract_ImplicitFtpUrl_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("ftp://files.example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("ftp://files.example.com", builder.RequestUri.OriginalString);
    }

    [Fact]
    public void TryExtract_ImplicitAbsoluteUri_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("mailto:test@example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
    }

    [Fact]
    public void TryExtract_ImplicitUrlWithHyphenPrefix_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "http://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
    }

    [Fact]
    public void TryExtract_ImplicitUrlInMiddleOfTokens_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("curl", "-X", "GET", "http://example.com", "-H", "Accept: json");

        context.Advance(3);
        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(4, context.CurrentIndex);
        Assert.Equal("-H", context.CurrentToken);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("http://example.com", builder.RequestUri.OriginalString);
    }

    [Fact]
    public void LooksLikeUrl_ReturnOK()
    {
        Assert.True(CurlUrlExtractor.LooksLikeUrl("http://example.com"));
        Assert.True(CurlUrlExtractor.LooksLikeUrl("https://example.com"));
        Assert.True(CurlUrlExtractor.LooksLikeUrl("ftp://example.com"));
        Assert.False(CurlUrlExtractor.LooksLikeUrl("mailto:test@example.com"));
        Assert.True(CurlUrlExtractor.LooksLikeUrl("/relative/path"));
        Assert.False(CurlUrlExtractor.LooksLikeUrl("just a string"));
        Assert.False(CurlUrlExtractor.LooksLikeUrl(null!));
        Assert.True(CurlUrlExtractor.LooksLikeUrl("/api/data"));
    }

    [Fact]
    public void TryExtract_ImplicitRelativeUrl_ReturnOK()
    {
        var extractor = new CurlUrlExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("/api/data");
        var result = extractor.TryExtract(builder, context);
        Assert.True(result);
        Assert.NotNull(builder.RequestUri);
        Assert.Equal("/api/data", builder.RequestUri.OriginalString);
    }
}