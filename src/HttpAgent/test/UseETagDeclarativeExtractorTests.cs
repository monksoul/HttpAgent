// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class UseETagDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(UseETagDeclarativeExtractor)));

        var extractor = new UseETagDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IUseETagDeclarativeExtractorTest1).GetMethod(
                nameof(IUseETagDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeParsingContext(method1, [],
            new HttpDeclarativeMetadata(method1, typeof(IUseETagDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new UseETagDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.False(httpRequestBuilder1.ETagEnabled);

        var method2 =
            typeof(IUseETagDeclarativeExtractorTest2).GetMethod(
                nameof(IUseETagDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeParsingContext(method2, [],
            new HttpDeclarativeMetadata(method2, typeof(IUseETagDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new UseETagDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.True(httpRequestBuilder2.ETagEnabled);

        var method3 =
            typeof(IUseETagDeclarativeExtractorTest2).GetMethod(
                nameof(IUseETagDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeParsingContext(method3, [],
            new HttpDeclarativeMetadata(method3, typeof(IUseETagDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new UseETagDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.False(httpRequestBuilder3.ETagEnabled);
    }
}

public interface IUseETagDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[UseETag]
public interface IUseETagDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [UseETag(false)]
    [Post("http://localhost:5000")]
    Task Test2();
}