// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpMultipartFormDataBuilderDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(
                typeof(HttpMultipartFormDataBuilderDeclarativeExtractor)));

        var extractor = new HttpMultipartFormDataBuilderDeclarativeExtractor();
        Assert.NotNull(extractor);
        Assert.Equal(2, extractor.Order);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        Action<HttpMultipartFormDataBuilder> builder = builder =>
        {
            builder.Add(new StringContent(""), "str", "text/plain");
        };
        var method1 =
            typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest1).GetMethod(
                nameof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [builder, builder],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new HttpMultipartFormDataBuilderDeclarativeExtractor().Extract(httpRequestBuilder1, context1));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        Action<HttpMultipartFormDataBuilder> builder = builder =>
        {
            builder.Add(new StringContent(""), "str", "text/plain");
        };

        var method1 =
            typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HttpMultipartFormDataBuilderDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.Timeout);

        var method2 =
            typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [builder],
            new HttpDeclarativeMethodMetadata(method2, typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HttpMultipartFormDataBuilderDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.MultipartFormDataBuilder);

        // 测试 [Multipart] 冲突问题
        var method3 =
            typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2).GetMethod(
                nameof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [new { }, builder],
            new HttpDeclarativeMethodMetadata(method3, typeof(IHttpMultipartFormDataBuilderDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        // ===
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        new HttpMultipartFormDataBuilderDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        // ===
        Assert.NotNull(httpRequestBuilder3.MultipartFormDataBuilder);
        Assert.Equal(2, httpRequestBuilder3.MultipartFormDataBuilder._partContents.Count);
    }
}

public interface IHttpMultipartFormDataBuilderDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1(Action<HttpMultipartFormDataBuilder> configure, Action<HttpMultipartFormDataBuilder> configure2);
}

public interface IHttpMultipartFormDataBuilderDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2(Action<HttpMultipartFormDataBuilder> configure);

    [Post("http://localhost:5000")]
    Task Test3([Multipart] object obj, Action<HttpMultipartFormDataBuilder> configure);
}