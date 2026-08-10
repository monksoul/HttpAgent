// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonUrlExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonUrlExtractor).GetProperty("PropertyName", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("url", prop!.GetValue(new JsonUrlExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonUrlExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "requestUri" }, prop!.GetValue(new JsonUrlExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"url\":\"https://example.com\"}");
        new JsonUrlExtractor().Extract(builder, context);
        Assert.Equal("https://example.com", builder.RequestUri?.OriginalString);
    }

    [Fact]
    public void Extract_EmptyUrl_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"url\":\"\"}");
        new JsonUrlExtractor().Extract(builder, context);
        Assert.Null(builder.RequestUri);
    }
}