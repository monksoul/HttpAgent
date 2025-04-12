// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpVersionDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(HttpVersionDeclarativeExtractor)));

        var extractor = new HttpVersionDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IVersionDeclarativeExtractorTest1).GetMethod(
                nameof(IVersionDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HttpVersionDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Version);

        var method2 =
            typeof(IVersionDeclarativeExtractorTest2).GetMethod(
                nameof(IVersionDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HttpVersionDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.Version);
        Assert.Equal("1.2", httpRequestBuilder2.Version.ToString());

        var method3 =
            typeof(IVersionDeclarativeExtractorTest2).GetMethod(
                nameof(IVersionDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new HttpVersionDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.Version);
        Assert.Equal("1.1", httpRequestBuilder3.Version.ToString());
    }
}

public interface IVersionDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[HttpVersion("1.2")]
public interface IVersionDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("api/test")]
    Task Test1();

    [HttpVersion("1.1")]
    [Post("api/test2")]
    Task Test2();
}