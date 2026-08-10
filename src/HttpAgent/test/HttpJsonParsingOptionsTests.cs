// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpJsonParsingOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new HttpJsonParsingOptions();
        Assert.NotNull(options.Extractors);
        Assert.Equal(13, options.Extractors.Count);
        Assert.Equal(
        [
            typeof(JsonMethodExtractor), typeof(JsonUrlExtractor), typeof(JsonBaseAddressExtractor),
            typeof(JsonHeadersExtractor), typeof(JsonParamsExtractor), typeof(JsonCookiesExtractor),
            typeof(JsonTimeoutExtractor), typeof(JsonClientExtractor), typeof(JsonVersionExtractor),
            typeof(JsonAuthExtractor), typeof(JsonDataExtractor), typeof(JsonMultipartExtractor),
            typeof(JsonProfilerExtractor)
        ], options.Extractors.Select(e => e.GetType()));
    }

    [Fact]
    public void RemoveExtractor_ReturnOK()
    {
        var options = new HttpJsonParsingOptions();
        options.RemoveExtractor<JsonMethodExtractor>();
        Assert.Equal(12, options.Extractors.Count);
        Assert.DoesNotContain(options.Extractors, e => e is JsonMethodExtractor);
    }

    [Fact]
    public void AddExtractor_Invalid_Parameters()
    {
        var options = new HttpJsonParsingOptions();
        Assert.Throws<ArgumentNullException>(() => options.AddExtractor(null!));
    }

    [Fact]
    public void AddExtractor_ReturnOK()
    {
        var options = new HttpJsonParsingOptions();
        options.AddExtractor(new JsonMethodExtractor());
        Assert.Equal(14, options.Extractors.Count);
        Assert.True(options.Extractors.Last() is JsonMethodExtractor);
    }

    [Fact]
    public void AddExtractors_Invalid_Parameters()
    {
        var options = new HttpJsonParsingOptions();
        Assert.Throws<ArgumentNullException>(() => options.AddExtractors(null!));
    }

    [Fact]
    public void AddExtractors_ReturnOK()
    {
        var options = new HttpJsonParsingOptions();
        options.AddExtractors(new JsonMethodExtractor(), new JsonUrlExtractor());
        Assert.Equal(15, options.Extractors.Count);
        Assert.True(options.Extractors[^2] is JsonMethodExtractor);
        Assert.True(options.Extractors.Last() is JsonUrlExtractor);
    }

    [Fact]
    public void GetDefaultExtractors_ReturnOK()
    {
        var extractors = HttpJsonParsingOptions.GetDefaultExtractors().ToList();
        Assert.Equal(13, extractors.Count);
        Assert.Equal(
        [
            typeof(JsonMethodExtractor), typeof(JsonUrlExtractor), typeof(JsonBaseAddressExtractor),
            typeof(JsonHeadersExtractor), typeof(JsonParamsExtractor), typeof(JsonCookiesExtractor),
            typeof(JsonTimeoutExtractor), typeof(JsonClientExtractor), typeof(JsonVersionExtractor),
            typeof(JsonAuthExtractor), typeof(JsonDataExtractor), typeof(JsonMultipartExtractor),
            typeof(JsonProfilerExtractor)
        ], extractors.Select(e => e.GetType()));
    }
}