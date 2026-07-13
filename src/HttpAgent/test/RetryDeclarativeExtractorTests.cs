// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RetryDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(RetryDeclarativeExtractor)));

        var extractor = new RetryDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IRetryDeclarativeExtractorTest1).GetMethod(
                nameof(IRetryDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IRetryDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new RetryDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.RetryOptions?.MaxRetries);

        var method2 =
            typeof(IRetryDeclarativeExtractorTest2).GetMethod(
                nameof(IRetryDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IRetryDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new RetryDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.RetryOptions?.MaxRetries);
        Assert.Equal(100, httpRequestBuilder2.RetryOptions?.MaxRetries);

        var method3 =
            typeof(IRetryDeclarativeExtractorTest2).GetMethod(
                nameof(IRetryDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IRetryDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new RetryDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.RetryOptions?.MaxRetries);
        Assert.Equal(200, httpRequestBuilder3.RetryOptions?.MaxRetries);
    }
}

public interface IRetryDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[Retry(100)]
public interface IRetryDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Retry(200)]
    [Post("http://localhost:5000")]
    Task Test2();
}