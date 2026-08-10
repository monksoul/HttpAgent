// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlVersionExtractorTests
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
    public void Flags_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var flagsProperty = typeof(CurlVersionExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-0", "--http1.0", "--http1.1", "--http2", "--http3"], flags);
    }

    [Fact]
    public void RequiresArgument_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var requiresArgumentProperty = typeof(CurlVersionExtractor).GetProperty("RequiresArgument",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(requiresArgumentProperty);
        var requiresArgument = (bool)requiresArgumentProperty.GetValue(extractor)!;
        Assert.False(requiresArgument);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.Version);
    }

    [Fact]
    public void TryExtract_Http10ShortFlag_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-0");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(1, 0), builder.Version);
    }

    [Fact]
    public void TryExtract_Http10LongFlag_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--http1.0");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(1, 0), builder.Version);
    }

    [Fact]
    public void TryExtract_Http11Flag_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--http1.1");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(1, 1), builder.Version);
    }

    [Fact]
    public void TryExtract_Http2Flag_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--http2");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(2, 0), builder.Version);
    }

    [Fact]
    public void TryExtract_Http3Flag_ReturnOK()
    {
        var extractor = new CurlVersionExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--http3");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.NotNull(builder.Version);
        Assert.Equal(new Version(3, 0), builder.Version);
    }
}