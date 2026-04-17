// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class SuppressExceptionsDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(SuppressExceptionsDeclarativeExtractor)));

        var extractor = new SuppressExceptionsDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(ISuppressExceptionsDeclarativeExtractorTest1).GetMethod(
                nameof(ISuppressExceptionsDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(ISuppressExceptionsDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new SuppressExceptionsDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.SuppressExceptionTypes);

        var method2 =
            typeof(ISuppressExceptionsDeclarativeExtractorTest2).GetMethod(
                nameof(ISuppressExceptionsDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(ISuppressExceptionsDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new SuppressExceptionsDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.SuppressExceptionTypes);
        Assert.Single(httpRequestBuilder2.SuppressExceptionTypes);

        var method3 =
            typeof(ISuppressExceptionsDeclarativeExtractorTest2).GetMethod(
                nameof(ISuppressExceptionsDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(ISuppressExceptionsDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new SuppressExceptionsDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.SuppressExceptionTypes);
        Assert.Equal([typeof(TimeoutException), typeof(TaskCanceledException)],
            httpRequestBuilder3.SuppressExceptionTypes);

        var method4 =
            typeof(ISuppressExceptionsDeclarativeExtractorTest2).GetMethod(
                nameof(ISuppressExceptionsDeclarativeExtractorTest2.Test3))!;
        var context4 = new HttpDeclarativeExtractorContext(method4, [],
            new HttpDeclarativeMethodMetadata(method4, typeof(ISuppressExceptionsDeclarativeExtractorTest2)));
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new SuppressExceptionsDeclarativeExtractor().Extract(httpRequestBuilder4, context4);
        Assert.Null(httpRequestBuilder4.SuppressExceptionTypes);
    }
}

public interface ISuppressExceptionsDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[SuppressExceptions]
public interface ISuppressExceptionsDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("api/test")]
    Task Test1();

    [SuppressExceptions(typeof(TimeoutException), typeof(TaskCanceledException))]
    [Post("api/test2")]
    Task Test2();

    [SuppressExceptions(false)]
    [Post("api/test")]
    Task Test3();
}