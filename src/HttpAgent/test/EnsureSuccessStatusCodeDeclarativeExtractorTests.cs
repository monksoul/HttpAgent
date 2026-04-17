// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class EnsureSuccessStatusCodeDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(EnsureSuccessStatusCodeDeclarativeExtractor)));

        var extractor = new EnsureSuccessStatusCodeDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest1).GetMethod(
                nameof(IEnsureSuccessStatusCodeDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new EnsureSuccessStatusCodeDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.EnsureSuccessStatusCodeEnabled);

        var method2 =
            typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2).GetMethod(
                nameof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new EnsureSuccessStatusCodeDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.EnsureSuccessStatusCodeEnabled);

        var method3 =
            typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2).GetMethod(
                nameof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IEnsureSuccessStatusCodeDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new EnsureSuccessStatusCodeDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.EnsureSuccessStatusCodeEnabled);
    }
}

public interface IEnsureSuccessStatusCodeDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[EnsureSuccessStatusCode]
public interface IEnsureSuccessStatusCodeDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [EnsureSuccessStatusCode(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}