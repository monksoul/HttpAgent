// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CurlHeadExtractorTests
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
        var extractor = new CurlHeadExtractor();
        var flagsProperty = typeof(CurlHeadExtractor).GetProperty("Flags",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(flagsProperty);
        var flags = (string[])flagsProperty.GetValue(extractor)!;
        Assert.Equal(["-I", "--head"], flags);
    }

    [Fact]
    public void RequiresArgument_ReturnOK()
    {
        var extractor = new CurlHeadExtractor();
        var requiresArgumentProperty = typeof(CurlHeadExtractor).GetProperty("RequiresArgument",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(requiresArgumentProperty);
        var requiresArgument = (bool)requiresArgumentProperty.GetValue(extractor)!;
        Assert.False(requiresArgument);
    }

    [Fact]
    public void TryExtract_NoMatch_ReturnOK()
    {
        var extractor = new CurlHeadExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-X", "POST");

        var result = extractor.TryExtract(builder, context);

        Assert.False(result);
        Assert.Equal(0, context.CurrentIndex);
        Assert.Null(builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_ShortFlag_ReturnOK()
    {
        var extractor = new CurlHeadExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-I", "http://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Equal("http://example.com", context.CurrentToken);
        Assert.Equal(HttpMethod.Head, builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_LongFlag_ReturnOK()
    {
        var extractor = new CurlHeadExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("--head", "http://example.com");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.Equal("http://example.com", context.CurrentToken);
        Assert.Equal(HttpMethod.Head, builder.HttpMethod);
    }

    [Fact]
    public void TryExtract_FlagAtEnd_ReturnOK()
    {
        var extractor = new CurlHeadExtractor();
        var builder = CreateBuilder();
        var context = CreateContext("-I");

        var result = extractor.TryExtract(builder, context);

        Assert.True(result);
        Assert.Equal(1, context.CurrentIndex);
        Assert.True(context.IsEndOfTokens);
        Assert.Equal(HttpMethod.Head, builder.HttpMethod);
    }
}