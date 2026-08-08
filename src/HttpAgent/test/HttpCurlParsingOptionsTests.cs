// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpCurlParsingOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new HttpCurlParsingOptions();
        Assert.NotNull(options.Extractors);
        Assert.Equal(12, options.Extractors.Count);
        Assert.Equal(
        [
            typeof(CurlMethodExtractor), typeof(CurlHeadExtractor), typeof(CurlHeaderExtractor),
            typeof(CurlCookieExtractor), typeof(CurlDataExtractor), typeof(CurlAuthExtractor),
            typeof(CurlUserAgentExtractor), typeof(CurlRefererExtractor), typeof(CurlFormExtractor),
            typeof(CurlTimeoutExtractor), typeof(CurlVersionExtractor), typeof(CurlUrlExtractor)
        ], options.Extractors.Select(u => u.GetType()).ToList());
    }

    [Fact]
    public void RemoveExtractor_ReturnOK()
    {
        var options = new HttpCurlParsingOptions();
        options.RemoveExtractor<CurlMethodExtractor>();
        Assert.Equal(11, options.Extractors.Count);
        Assert.DoesNotContain(options.Extractors, u => u is CurlMethodExtractor);
    }

    [Fact]
    public void AddExtractor_Invalid_Parameters()
    {
        var options = new HttpCurlParsingOptions();
        Assert.Throws<ArgumentNullException>(() => options.AddExtractor(null!));
    }

    [Fact]
    public void AddExtractor_ReturnOK()
    {
        var options = new HttpCurlParsingOptions();
        options.AddExtractor(new CurlMethodExtractor());
        Assert.Equal(13, options.Extractors.Count);
        Assert.True(options.Extractors.Last() is CurlMethodExtractor);
    }

    [Fact]
    public void AddExtractors_Invalid_Parameters()
    {
        var options = new HttpCurlParsingOptions();
        Assert.Throws<ArgumentNullException>(() => options.AddExtractors(null!));
    }

    [Fact]
    public void AddExtractors_ReturnOK()
    {
        var options = new HttpCurlParsingOptions();
        options.AddExtractors(new CurlMethodExtractor(), new CurlUrlExtractor());
        Assert.Equal(14, options.Extractors.Count);
        Assert.True(options.Extractors.Last() is CurlUrlExtractor);
    }

    [Fact]
    public void GetDefaultExtractors_ReturnOK()
    {
        var extractors = HttpCurlParsingOptions.GetDefaultExtractors().ToList();

        Assert.Equal(12, extractors.Count);
        Assert.Equal(
        [
            typeof(CurlMethodExtractor), typeof(CurlHeadExtractor), typeof(CurlHeaderExtractor),
            typeof(CurlCookieExtractor), typeof(CurlDataExtractor), typeof(CurlAuthExtractor),
            typeof(CurlUserAgentExtractor), typeof(CurlRefererExtractor), typeof(CurlFormExtractor),
            typeof(CurlTimeoutExtractor), typeof(CurlVersionExtractor), typeof(CurlUrlExtractor)
        ], extractors.Select(u => u.GetType()).ToList());
    }
}