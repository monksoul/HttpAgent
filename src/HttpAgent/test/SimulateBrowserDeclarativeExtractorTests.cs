// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class SimulateBrowserDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(SimulateBrowserDeclarativeExtractor)));

        var extractor = new SimulateBrowserDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(ISimulateBrowserDeclarativeExtractorTest1).GetMethod(
                nameof(ISimulateBrowserDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new SimulateBrowserDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Headers);

        var method2 =
            typeof(ISimulateBrowserDeclarativeExtractorTest2).GetMethod(
                nameof(ISimulateBrowserDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new SimulateBrowserDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.Headers);
        Assert.Single(httpRequestBuilder2.Headers);
        Assert.Equal("User-Agent", httpRequestBuilder2.Headers.Keys.First());

        var method3 =
            typeof(ISimulateBrowserDeclarativeExtractorTest2).GetMethod(
                nameof(ISimulateBrowserDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new SimulateBrowserDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.Headers);
        Assert.Single(httpRequestBuilder3.Headers);
        Assert.Equal("User-Agent", httpRequestBuilder3.Headers.Keys.First());
        Assert.Equal(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0",
            httpRequestBuilder3.Headers.Values.First().First());

        var method4 =
            typeof(ISimulateBrowserDeclarativeExtractorTest2).GetMethod(
                nameof(ISimulateBrowserDeclarativeExtractorTest2.Test3))!;
        var context4 = new HttpDeclarativeExtractorContext(method4, []);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new SimulateBrowserDeclarativeExtractor().Extract(httpRequestBuilder4, context4);
        Assert.NotNull(httpRequestBuilder4.Headers);
        Assert.Single(httpRequestBuilder4.Headers);
        Assert.Equal("User-Agent", httpRequestBuilder4.Headers.Keys.First());
        Assert.Equal(
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Mobile Safari/537.36 Edg/142.0.0.0",
            httpRequestBuilder4.Headers.Values.First().First());
    }
}

public interface ISimulateBrowserDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[SimulateBrowser]
public interface ISimulateBrowserDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [SimulateBrowser]
    Task Test2();

    [SimulateBrowser(Mobile = true)]
    Task Test3();
}