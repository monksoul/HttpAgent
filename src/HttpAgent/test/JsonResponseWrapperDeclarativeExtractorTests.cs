// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonResponseWrapperDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(JsonResponseWrapperDeclarativeExtractor)));

        var extractor = new JsonResponseWrapperDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IJsonResponseWrapperDeclarativeExtractorTest1).GetMethod(
                nameof(IJsonResponseWrapperDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IJsonResponseWrapperDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrapperDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.JsonResponseWrapperEnabled);

        var method2 =
            typeof(IJsonResponseWrapperDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseWrapperDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IJsonResponseWrapperDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrapperDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.JsonResponseWrapperEnabled);

        var method3 =
            typeof(IJsonResponseWrapperDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseWrapperDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IJsonResponseWrapperDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrapperDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.JsonResponseWrapperEnabled);
    }
}

public interface IJsonResponseWrapperDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[JsonResponseWrapper]
public interface IJsonResponseWrapperDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [JsonResponseWrapper(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}