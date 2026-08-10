// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonParamsExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonParamsExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("params", prop!.GetValue(new JsonParamsExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonParamsExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "queries", "query", "queryParameters" }, prop!.GetValue(new JsonParamsExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"queries\":{\"page\":1,\"size\":10}}");
        new JsonParamsExtractor().Extract(builder, context);
        Assert.NotNull(builder.QueryParameters);
        Assert.True(builder.QueryParameters!.ContainsKey("page"));
    }
}