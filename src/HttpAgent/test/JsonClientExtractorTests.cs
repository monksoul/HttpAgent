// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonClientExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonClientExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("client", prop!.GetValue(new JsonClientExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonClientExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "clientName", "httpClientName" }, prop!.GetValue(new JsonClientExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"client\":\"myclient\"}");
        new JsonClientExtractor().Extract(builder, context);
        Assert.Equal("myclient", builder.HttpClientName);
    }

    [Fact]
    public void Extract_NullValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"client\":null}");
        new JsonClientExtractor().Extract(builder, context);
        Assert.Null(builder.HttpClientName);
    }
}