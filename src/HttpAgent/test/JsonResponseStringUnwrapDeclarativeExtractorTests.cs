// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonResponseStringUnwrapDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(JsonResponseStringUnwrapDeclarativeExtractor)));

        var extractor = new JsonResponseStringUnwrapDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest1).GetMethod(
                nameof(IJsonResponseStringUnwrapDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseStringUnwrapDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.JsonResponseStringUnwrapEnabled);

        var method2 =
            typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseStringUnwrapDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseStringUnwrapDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.JsonResponseStringUnwrapEnabled);

        var method3 =
            typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseStringUnwrapDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IJsonResponseStringUnwrapDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseStringUnwrapDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.JsonResponseStringUnwrapEnabled);
    }
}

public interface IJsonResponseStringUnwrapDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[JsonResponseStringUnwrap]
public interface IJsonResponseStringUnwrapDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [JsonResponseStringUnwrap(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}