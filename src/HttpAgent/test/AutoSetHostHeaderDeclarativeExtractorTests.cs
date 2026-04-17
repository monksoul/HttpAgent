// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class AutoSetHostHeaderDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(AutoSetHostHeaderDeclarativeExtractor)));

        var extractor = new AutoSetHostHeaderDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IAutoSetHostHeaderDeclarativeExtractorTest1).GetMethod(
                nameof(IAutoSetHostHeaderDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IAutoSetHostHeaderDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new AutoSetHostHeaderDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.AutoSetHostHeaderEnabled);

        var method2 =
            typeof(IAutoSetHostHeaderDeclarativeExtractorTest2).GetMethod(
                nameof(IAutoSetHostHeaderDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IAutoSetHostHeaderDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new AutoSetHostHeaderDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.AutoSetHostHeaderEnabled);

        var method3 =
            typeof(IAutoSetHostHeaderDeclarativeExtractorTest2).GetMethod(
                nameof(IAutoSetHostHeaderDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IAutoSetHostHeaderDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new AutoSetHostHeaderDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.AutoSetHostHeaderEnabled);
    }
}

public interface IAutoSetHostHeaderDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[AutoSetHostHeader]
public interface IAutoSetHostHeaderDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [AutoSetHostHeader(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}