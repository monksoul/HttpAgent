// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DisableCacheDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(DisableCacheDeclarativeExtractor)));

        var extractor = new DisableCacheDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IDisableCacheDeclarativeExtractorTest1).GetMethod(
                nameof(IDisableCacheDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IDisableCacheDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new DisableCacheDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.DisableCacheEnabled);

        var method2 =
            typeof(IDisableCacheDeclarativeExtractorTest2).GetMethod(
                nameof(IDisableCacheDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IDisableCacheDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new DisableCacheDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.DisableCacheEnabled);

        var method3 =
            typeof(IDisableCacheDeclarativeExtractorTest2).GetMethod(
                nameof(IDisableCacheDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IDisableCacheDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new DisableCacheDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.DisableCacheEnabled);
    }
}

public interface IDisableCacheDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[DisableCache]
public interface IDisableCacheDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [DisableCache(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}