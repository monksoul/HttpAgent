// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlHeaderExtractorTests
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
        var extractor = new CurlHeaderExtractor();
        var flagsProperty = typeof(CurlHeaderExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-H", "--header"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.Headers);
        Assert.Null(builder.ContentType);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.Headers);
    }

    [Fact]
    public void TryExtract_ValidHeader_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H", "X-Custom: value");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.Headers);
        Assert.True(builder.Headers.ContainsKey("X-Custom"));
        Assert.Equal("value", builder.Headers["X-Custom"][0]);
    }

    [Fact]
    public void TryExtract_ContentTypeHeader_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--header", "Content-Type: application/json");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("application/json", builder.ContentType);
    }

    [Fact]
    public void TryExtract_ContentTypeHeaderWithCharset_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H", "Content-Type: text/plain; charset=utf-8");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Equal("text/plain", builder.ContentType);
        Assert.NotNull(builder.ContentEncoding);
        Assert.Equal(Encoding.UTF8, builder.ContentEncoding);
    }

    [Fact]
    public void TryExtract_ContentTypeHeaderEmptyValue_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H", "Content-Type:");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Null(builder.ContentType);
        Assert.Null(builder.Headers);
        Assert.NotNull(builder.ContentHeaders);
        Assert.True(builder.ContentHeaders.ContainsKey("Content-Type"));
        Assert.Equal(string.Empty, builder.ContentHeaders["Content-Type"].First());
    }

    [Fact]
    public void TryExtract_InvalidHeaderFormat_Invalid_Parameters()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H", "invalid");

        var exception = Assert.Throws<ArgumentException>(() => extractor.TryExtract(builder, context));
        Assert.Contains("Invalid header format", exception.Message);
    }

    [Fact]
    public void TryExtract_LongFlagWithValidHeader_ReturnOK()
    {
        var extractor = new CurlHeaderExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--header", "Accept: application/json");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.NotNull(builder.Headers);
        Assert.True(builder.Headers.ContainsKey("Accept"));
        Assert.Equal("application/json", builder.Headers["Accept"][0]);
    }
}