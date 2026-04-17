// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(
                typeof(HttpRequestBuilderDeclarativeExtractor)));

        var extractor = new HttpRequestBuilderDeclarativeExtractor();
        Assert.NotNull(extractor);
        Assert.Equal(1, extractor.Order);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        Action<HttpRequestBuilder> builder = builder =>
        {
            builder.SetTimeout(1000);
        };
        var method1 =
            typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest1).GetMethod(
                nameof(IHttpRequestBuilderConfigureDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [builder, builder],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new HttpRequestBuilderDeclarativeExtractor().Extract(httpRequestBuilder1, context1));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var builder = (HttpRequestBuilder builder) =>
        {
            builder.SetTimeout(1000);
        };

        var method1 =
            typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HttpRequestBuilderDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Timeout);

        var method2 =
            typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [builder],
            new HttpDeclarativeMethodMetadata(method2, typeof(IHttpRequestBuilderConfigureDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HttpRequestBuilderDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), httpRequestBuilder2.Timeout);
    }
}

public interface IHttpRequestBuilderConfigureDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1(Action<HttpRequestBuilder> configure, Action<HttpRequestBuilder> configure2);
}

public interface IHttpRequestBuilderConfigureDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2(Action<HttpRequestBuilder> configure);
}