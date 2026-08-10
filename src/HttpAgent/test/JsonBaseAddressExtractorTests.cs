// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonBaseAddressExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonBaseAddressExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("baseURL", prop!.GetValue(new JsonBaseAddressExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonBaseAddressExtractor).GetProperty("Aliases",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "baseAddress", "baseUrl" }, prop!.GetValue(new JsonBaseAddressExtractor()));
    }

    [Fact]
    public void Extract_ValidString_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"baseURL\":\"https://example.com\"}");
        new JsonBaseAddressExtractor().Extract(builder, context);
        Assert.Equal("https://example.com", builder.BaseAddress?.OriginalString);
    }

    [Fact]
    public void Extract_Alias_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"baseAddress\":\"https://alias.com\"}");
        new JsonBaseAddressExtractor().Extract(builder, context);
        Assert.Equal("https://alias.com", builder.BaseAddress?.OriginalString);
    }

    [Fact]
    public void Extract_EmptyString_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"baseURL\":\"\"}");
        new JsonBaseAddressExtractor().Extract(builder, context);
        Assert.Null(builder.BaseAddress);
    }

    [Fact]
    public void Extract_NotJsonValue_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"baseURL\":{\"nested\":1}}");
        new JsonBaseAddressExtractor().Extract(builder, context);
        Assert.Null(builder.BaseAddress);
    }
}