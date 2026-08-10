// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonVersionExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonVersionExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("httpVersion", prop!.GetValue(new JsonVersionExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonVersionExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "version" }, prop!.GetValue(new JsonVersionExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"httpVersion\":\"1.1\"}");
        new JsonVersionExtractor().Extract(builder, context);
        Assert.Equal(new Version(1, 1), builder.Version);
    }

    [Fact]
    public void Extract_InvalidVersion_Invalid_Parameters()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"httpVersion\":\"abc\"}");
        Assert.Throws<ArgumentException>(() => new JsonVersionExtractor().Extract(builder, context));
    }
}