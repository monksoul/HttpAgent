// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.Net.Http.Headers;

namespace HttpAgent.Tests;

public class CurlRefererExtractorTests
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
        var extractor = new CurlRefererExtractor();
        var flagsProperty = typeof(CurlRefererExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-e", "--referer"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlRefererExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.Headers);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlRefererExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-e");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.Headers);
    }

    [Fact]
    public void TryExtract_ValidReferer_ReturnOK()
    {
        var extractor = new CurlRefererExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-e", "https://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.Headers);
        Assert.True(builder.Headers.ContainsKey(HeaderNames.Referer));
        Assert.Equal("https://example.com", builder.Headers[HeaderNames.Referer][0]);
    }

    [Fact]
    public void TryExtract_LongFlagValidReferer_ReturnOK()
    {
        var extractor = new CurlRefererExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--referer", "https://long.example.com/path");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.Headers);
        Assert.True(builder.Headers.ContainsKey(HeaderNames.Referer));
        Assert.Equal("https://long.example.com/path", builder.Headers[HeaderNames.Referer][0]);
    }
}