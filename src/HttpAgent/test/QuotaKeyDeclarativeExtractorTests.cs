// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class QuotaKeyDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(QuotaKeyDeclarativeExtractor)));

        var extractor = new QuotaKeyDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IQuotaKeyDeclarativeExtractorTest1).GetMethod(
                nameof(IQuotaKeyDeclarativeExtractorTest1.Test1))!;
        var context1 = new HttpDeclarativeParsingContext(method1, [],
            new HttpDeclarativeMetadata(method1, typeof(IQuotaKeyDeclarativeExtractorTest1)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new QuotaKeyDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.QuotaKey);

        var method2 =
            typeof(IQuotaKeyDeclarativeExtractorTest2).GetMethod(
                nameof(IQuotaKeyDeclarativeExtractorTest2.Test1))!;
        var context2 = new HttpDeclarativeParsingContext(method2, [],
            new HttpDeclarativeMetadata(method2, typeof(IQuotaKeyDeclarativeExtractorTest2)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new QuotaKeyDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.Null(httpRequestBuilder2.QuotaKey);

        var method3 =
            typeof(IQuotaKeyDeclarativeExtractorTest2).GetMethod(
                nameof(IQuotaKeyDeclarativeExtractorTest2.Test2))!;
        var context3 = new HttpDeclarativeParsingContext(method3, [],
            new HttpDeclarativeMetadata(method3, typeof(IQuotaKeyDeclarativeExtractorTest2)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new QuotaKeyDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.Equal("github", httpRequestBuilder3.QuotaKey);
    }
}

public interface IQuotaKeyDeclarativeExtractorTest1 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();
}

[QuotaKey(null)]
public interface IQuotaKeyDeclarativeExtractorTest2 : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [QuotaKey("github")]
    [Post("http://localhost:5000")]
    Task Test2();
}