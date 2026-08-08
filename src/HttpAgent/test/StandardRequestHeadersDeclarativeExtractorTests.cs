// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StandardRequestHeadersDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(StandardRequestHeadersDeclarativeExtractor)));

        var extractor = new StandardRequestHeadersDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IUseStandardRequestHeadersDeclarativeExtractorTest1).GetMethod(
                nameof(IUseStandardRequestHeadersDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IUseStandardRequestHeadersDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new StandardRequestHeadersDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.StandardRequestHeadersEnabled);

        var method2 =
            typeof(IUseStandardRequestHeadersDeclarativeExtractorTest2).GetMethod(
                nameof(IUseStandardRequestHeadersDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IUseStandardRequestHeadersDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new StandardRequestHeadersDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.StandardRequestHeadersEnabled);

        var method3 =
            typeof(IUseStandardRequestHeadersDeclarativeExtractorTest2).GetMethod(
                nameof(IUseStandardRequestHeadersDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IUseStandardRequestHeadersDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new StandardRequestHeadersDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.StandardRequestHeadersEnabled);
    }
}

public interface IUseStandardRequestHeadersDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[StandardRequestHeaders]
public interface IUseStandardRequestHeadersDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [StandardRequestHeaders(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}