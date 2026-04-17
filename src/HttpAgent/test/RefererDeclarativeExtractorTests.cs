// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RefererDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(RefererDeclarativeExtractor)));

        var extractor = new RefererDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IRefererDeclarativeExtractorTest1).GetMethod(
                nameof(IRefererDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IRefererDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new RefererDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Headers);

        var method2 =
            typeof(IRefererDeclarativeExtractorTest2).GetMethod(
                nameof(IRefererDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IRefererDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new RefererDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.Headers);
        Assert.Equal("http://localhost:5000", httpRequestBuilder2.Headers["Referer"].First());

        var method3 =
            typeof(IRefererDeclarativeExtractorTest2).GetMethod(
                nameof(IRefererDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IRefererDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new RefererDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.Headers);
        Assert.Equal("https://furion.net/", httpRequestBuilder3.Headers["Referer"].First());
    }
}

public interface IRefererDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[Referer("http://localhost:5000")]
public interface IRefererDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("api/test")]
    Task Test1();

    [Referer("https://furion.net/")]
    [Post("api/test2")]
    Task Test2();
}