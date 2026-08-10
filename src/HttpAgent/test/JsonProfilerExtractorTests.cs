// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonProfilerExtractorTests
{
    private static HttpRequestBuilder CreateBuilder() =>
        (HttpRequestBuilder)typeof(HttpRequestBuilder).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)!.Invoke(null);

    private static HttpJsonParsingContext CreateContext(string json) =>
        new(JsonNode.Parse(json)!.AsObject());

    [Fact]
    public void PropertyName_ReturnOK()
    {
        var prop = typeof(JsonProfilerExtractor).GetProperty("PropertyName",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal("profiler", prop!.GetValue(new JsonProfilerExtractor()));
    }

    [Fact]
    public void Aliases_ReturnOK()
    {
        var prop = typeof(JsonProfilerExtractor).GetProperty("Aliases", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(new[] { "debugger" }, prop!.GetValue(new JsonProfilerExtractor()));
    }

    [Fact]
    public void Extract_Enable_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"profiler\":true}");
        new JsonProfilerExtractor().Extract(builder, context);
        Assert.True(builder.ProfilerEnabled);
    }

    [Fact]
    public void Extract_Disable_ReturnOK()
    {
        var builder = CreateBuilder();
        var context = CreateContext("{\"profiler\":false}");
        new JsonProfilerExtractor().Extract(builder, context);
        Assert.False(builder.ProfilerEnabled);
        Assert.True(builder.ProfilerDisabled);
    }
}