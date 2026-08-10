// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonCookiesExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonCookiesExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("cookies", prop!.GetValue(new JsonCookiesExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"cookies\":{\"session\":\"abc\",\"user\":\"john\"}}");
        new JsonCookiesExtractor().Extract(builder, context);
        Assert.NotNull(builder.Cookies);
        Assert.Equal("abc", builder.Cookies!["session"]);
        Assert.Equal("john", builder.Cookies["user"]);
    }

    [Fact]
    public void Extract_EmptyObject_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"cookies\":{}}");
        new JsonCookiesExtractor().Extract(builder, context);
        Assert.NotNull(builder.Cookies);
        Assert.Empty(builder.Cookies);
    }
}