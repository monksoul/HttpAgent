// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RequestEventHandlerDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(RequestEventHandlerDeclarativeExtractor)));

        var extractor = new RequestEventHandlerDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IEventHandlerDeclarativeExtractorTest1).GetMethod(
                nameof(IEventHandlerDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new RequestEventHandlerDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.RequestEventHandlerType);

        var method2 =
            typeof(IEventHandlerDeclarativeExtractorTest2).GetMethod(
                nameof(IEventHandlerDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new RequestEventHandlerDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
        Assert.Equal(typeof(MyRequestEventHandler), httpRequestBuilder2.RequestEventHandlerType);

        var method3 =
            typeof(IEventHandlerDeclarativeExtractorTest2).GetMethod(
                nameof(IEventHandlerDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new RequestEventHandlerDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.RequestEventHandlerType);
        Assert.Equal(typeof(MyRequestEventHandler2), httpRequestBuilder3.RequestEventHandlerType);
    }
}

public interface IEventHandlerDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[RequestEventHandler(typeof(MyRequestEventHandler))]
public interface IEventHandlerDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [RequestEventHandler<MyRequestEventHandler2>]
    [Post("http://localhost:5000")]
    Task Test2();
}

public class MyRequestEventHandler : IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) { }

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) { }

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) { }
}

public class MyRequestEventHandler2 : IHttpRequestEventHandler
{
    /// <inheritdoc />
    public void OnPreSendRequest(HttpRequestMessage httpRequestMessage) { }

    /// <inheritdoc />
    public void OnPostReceiveResponse(HttpResponseMessage httpResponseMessage) { }

    /// <inheritdoc />
    public void OnRequestFailed(Exception exception, HttpResponseMessage? httpResponseMessage) { }
}