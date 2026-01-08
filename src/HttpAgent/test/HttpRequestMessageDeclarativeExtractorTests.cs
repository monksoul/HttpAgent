// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestMessageDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(
                typeof(HttpRequestMessageDeclarativeExtractor)));

        var extractor = new HttpRequestMessageDeclarativeExtractor();
        Assert.NotNull(extractor);
        Assert.Equal(4, extractor.Order);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        Action<HttpRequestMessage> builder = _ =>
        {
        };
        var method1 =
            typeof(IHttpRequestMessageConfigureDeclarativeExtractorTest1).GetMethod(
                nameof(IHttpRequestMessageConfigureDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [builder, builder]);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new HttpRequestMessageDeclarativeExtractor().Extract(httpRequestBuilder1, context1));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = (HttpRequestMessage builder) => { };

        var method1 =
            typeof(IHttpRequestMessageConfigureDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpRequestMessageConfigureDeclarativeExtractorTest2.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HttpRequestMessageDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.OnPreSendRequest);

        var method2 =
            typeof(IHttpRequestMessageConfigureDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpRequestMessageConfigureDeclarativeExtractorTest2.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [builder]);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HttpRequestMessageDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.OnPreSendRequest);
    }
}

public interface IHttpRequestMessageConfigureDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1(Action<HttpRequestMessage> configure, Action<HttpRequestMessage> configure2);
}

public interface IHttpRequestMessageConfigureDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2(Action<HttpRequestMessage> configure);
}