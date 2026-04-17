// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpClientNameDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(HttpClientNameDeclarativeExtractor)));

        var extractor = new HttpClientNameDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IHttpClientNameDeclarativeExtractorTest1).GetMethod(
                nameof(IHttpClientNameDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHttpClientNameDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HttpClientNameDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.HttpClientName);

        var method2 =
            typeof(IHttpClientNameDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpClientNameDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IHttpClientNameDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HttpClientNameDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.Null(httpRequestBuilder2.HttpClientName);

        var method3 =
            typeof(IHttpClientNameDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpClientNameDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IHttpClientNameDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new HttpClientNameDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.Equal("github", httpRequestBuilder3.HttpClientName);
    }
}

public interface IHttpClientNameDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[HttpClientName(null)]
public interface IHttpClientNameDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [HttpClientName("github")]
    [Post("http://localhost:5000")]
    Task Test2();
}