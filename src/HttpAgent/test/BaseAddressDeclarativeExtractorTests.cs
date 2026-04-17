// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class BaseAddressDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(BaseAddressDeclarativeExtractor)));

        var extractor = new BaseAddressDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IBaseAddressDeclarativeExtractorTest1).GetMethod(
                nameof(IBaseAddressDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IBaseAddressDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new BaseAddressDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.BaseAddress);

        var method2 =
            typeof(IBaseAddressDeclarativeExtractorTest2).GetMethod(
                nameof(IBaseAddressDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IBaseAddressDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new BaseAddressDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.BaseAddress);
        Assert.Equal("http://localhost:5000/", httpRequestBuilder2.BaseAddress.ToString());

        var method3 =
            typeof(IBaseAddressDeclarativeExtractorTest2).GetMethod(
                nameof(IBaseAddressDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IBaseAddressDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new BaseAddressDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.BaseAddress);
        Assert.Equal("https://furion.net/", httpRequestBuilder3.BaseAddress.ToString());
    }
}

public interface IBaseAddressDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[BaseAddress("http://localhost:5000")]
public interface IBaseAddressDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("api/test")]
    Task Test1();

    [BaseAddress("https://furion.net/")]
    [Post("api/test2")]
    Task Test2();
}