// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class TraceIdentifierDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(TraceIdentifierDeclarativeExtractor)));

        var extractor = new TraceIdentifierDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(ITraceIdentifierDeclarativeExtractorTest1).GetMethod(
                nameof(ITraceIdentifierDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(ITraceIdentifierDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new TraceIdentifierDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.TraceIdentifier);

        var method2 =
            typeof(ITraceIdentifierDeclarativeExtractorTest2).GetMethod(
                nameof(ITraceIdentifierDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(ITraceIdentifierDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new TraceIdentifierDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.TraceIdentifier);
        Assert.Equal("furion-remote", httpRequestBuilder2.TraceIdentifier);

        var method3 =
            typeof(ITraceIdentifierDeclarativeExtractorTest2).GetMethod(
                nameof(ITraceIdentifierDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(ITraceIdentifierDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new TraceIdentifierDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.TraceIdentifier);
        Assert.Equal("http-agent", httpRequestBuilder3.TraceIdentifier);
    }
}

public interface ITraceIdentifierDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[TraceIdentifier("furion-remote")]
public interface ITraceIdentifierDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [TraceIdentifier("http-agent")]
    [Post("http://localhost:5000")]
    Task Test2();
}