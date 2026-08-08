// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlMethodExtractorTests
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
        var extractor = new CurlMethodExtractor();
        var flagsProperty = typeof(CurlMethodExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-X", "--request"], flags);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlMethodExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-H", "Content-Type: text/html");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_FlagWithoutArgument_ReturnOK()
    {
        var extractor = new CurlMethodExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Null(builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_ValidMethod_ReturnOK()
    {
        var extractor = new CurlMethodExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.HttpMethod);
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_LongFlagValidMethod_ReturnOK()
    {
        var extractor = new CurlMethodExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--request", "PUT");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(2, context.CurrentIndex);
        Assert.NotNull(builder.HttpMethod);
        Assert.Equal(HttpMethod.Put, builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_InvalidMethod_Invalid_Parameters()
    {
        var extractor = new CurlMethodExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "INVALID");

        var result = extractor.TryExtract(builder, context);
        Assert.True(result);
        Assert.NotNull(builder.HttpMethod);
        Assert.Equal("INVALID", builder.HttpMethod.ToString());
    }
}