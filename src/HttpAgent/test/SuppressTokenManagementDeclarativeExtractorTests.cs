// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class SuppressTokenManagementDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(SuppressTokenManagementDeclarativeExtractor)));

        var extractor = new SuppressTokenManagementDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(ISuppressTokenManagementDeclarativeExtractorTest1).GetMethod(
                nameof(ISuppressTokenManagementDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(ISuppressTokenManagementDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new SuppressTokenManagementDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.SuppressTokenManagement);

        var method2 =
            typeof(ISuppressTokenManagementDeclarativeExtractorTest1).GetMethod(
                nameof(ISuppressTokenManagementDeclarativeExtractorTest1.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(ISuppressTokenManagementDeclarativeExtractorTest1)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new SuppressTokenManagementDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.SuppressTokenManagement);
    }
}

public interface ISuppressTokenManagementDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [SuppressTokenManagement]
    [Post("api/test2")]
    Task Test2();
}