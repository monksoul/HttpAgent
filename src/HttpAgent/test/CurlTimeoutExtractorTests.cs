// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlTimeoutExtractorTests
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
        var extractor = new CurlTimeoutExtractor();
        var flagsProperty = typeof(CurlTimeoutExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-m", "--max-time"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.TimeoutOptions?.Timeout);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-m");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.TimeoutOptions?.Timeout);
    }

    [Fact]
    public void TryExtract_ValidIntegerSeconds_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-m", "30");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.TimeoutOptions?.Timeout);
        Assert.Equal(TimeSpan.FromSeconds(30), builder.TimeoutOptions!.Timeout);
    }

    [Fact]
    public void TryExtract_ValidDecimalSeconds_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--max-time", "12.5");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.TimeoutOptions?.Timeout);
        Assert.Equal(TimeSpan.FromSeconds(12.5), builder.TimeoutOptions!.Timeout);
    }

    [Fact]
    public void TryExtract_InvalidNumber_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-m", "abc");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Null(builder.TimeoutOptions?.Timeout);
    }

    [Fact]
    public void TryExtract_NegativeSeconds_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-m", "-5");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Null(builder.TimeoutOptions?.Timeout);
    }

    [Fact]
    public void TryExtract_ZeroSeconds_ReturnOK()
    {
        var extractor = new CurlTimeoutExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-m", "0");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.Null(builder.TimeoutOptions?.Timeout);
    }
}