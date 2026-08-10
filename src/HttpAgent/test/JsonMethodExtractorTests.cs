// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonMethodExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonMethodExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("method", prop!.GetValue(new JsonMethodExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"method\":\"POST\"}");
        new JsonMethodExtractor().Extract(builder, context);
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
    }

    [Fact]
    public void Extract_CustomMethod_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"method\":\"CUSTOM\"}");
        new JsonMethodExtractor().Extract(builder, context);
        Assert.NotNull(builder.HttpMethod);
        Assert.Equal("CUSTOM", builder.HttpMethod.ToString());
    }

    [Fact]
    public void Extract_EmptyMethod_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"method\":\"\"}");
        new JsonMethodExtractor().Extract(builder, context);
        Assert.Null(builder.HttpMethod);
    }
}