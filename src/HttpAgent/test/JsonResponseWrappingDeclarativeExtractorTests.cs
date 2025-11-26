// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class JsonResponseWrappingDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(JsonResponseWrappingDeclarativeExtractor)));

        var extractor = new JsonResponseWrappingDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IJsonResponseWrappingDeclarativeExtractorTest1).GetMethod(
                nameof(IJsonResponseWrappingDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrappingDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.__Enable__JsonResponseWrapping__);

        var method2 =
            typeof(IJsonResponseWrappingDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseWrappingDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrappingDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.__Enable__JsonResponseWrapping__);

        var method3 =
            typeof(IJsonResponseWrappingDeclarativeExtractorTest2).GetMethod(
                nameof(IJsonResponseWrappingDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new JsonResponseWrappingDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.__Enable__JsonResponseWrapping__);
    }
}

public interface IJsonResponseWrappingDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[JsonResponseWrapping]
public interface IJsonResponseWrappingDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [JsonResponseWrapping(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}