// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonTimeoutExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonTimeoutExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("timeout", prop!.GetValue(new JsonTimeoutExtractor()));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"timeout\":5000}");
        new JsonTimeoutExtractor().Extract(builder, context);
        Assert.NotNull(builder.TimeoutOptions);
        Assert.Equal(TimeSpan.FromMilliseconds(5000), builder.TimeoutOptions!.Timeout);
    }

    [Fact]
    public void Extract_NotNumeric_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"timeout\":\"abc\"}");
        new JsonTimeoutExtractor().Extract(builder, context);
        Assert.Null(builder.TimeoutOptions);
    }
}