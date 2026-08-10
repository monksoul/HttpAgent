// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RemoveTrailingSlashDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(RemoveTrailingSlashDeclarativeExtractor)));

        var extractor = new RemoveTrailingSlashDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IRemoveTrailingSlashDeclarativeExtractorTest1).GetMethod(
                nameof(IRemoveTrailingSlashDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeParsingContext(method1, [],
            new HttpDeclarativeMetadata(method1, typeof(IRemoveTrailingSlashDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new RemoveTrailingSlashDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.RemoveTrailingSlashEnabled);

        var method2 =
            typeof(IRemoveTrailingSlashDeclarativeExtractorTest2).GetMethod(
                nameof(IRemoveTrailingSlashDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeParsingContext(method2, [],
            new HttpDeclarativeMetadata(method2, typeof(IRemoveTrailingSlashDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new RemoveTrailingSlashDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.RemoveTrailingSlashEnabled);

        var method3 =
            typeof(IRemoveTrailingSlashDeclarativeExtractorTest2).GetMethod(
                nameof(IRemoveTrailingSlashDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeParsingContext(method3, [],
            new HttpDeclarativeMetadata(method3, typeof(IRemoveTrailingSlashDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new RemoveTrailingSlashDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.RemoveTrailingSlashEnabled);
    }
}

public interface IRemoveTrailingSlashDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[RemoveTrailingSlash]
public interface IRemoveTrailingSlashDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [RemoveTrailingSlash(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}