// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HeaderDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(HeaderDeclarativeExtractor)));

        var extractor = new HeaderDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IHeaderDeclarativeTest).GetMethod(nameof(IHeaderDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder1, context1);

        Assert.NotNull(httpRequestBuilder1.Headers);
        Assert.Equal(2, httpRequestBuilder1.Headers.Count);
        Assert.Equal("value1", httpRequestBuilder1.Headers["header1"].First());
        Assert.Equal("value2", httpRequestBuilder1.Headers["header2"].First());
        Assert.NotNull(httpRequestBuilder1.HeadersToRemove);
        Assert.Equal(2, httpRequestBuilder1.HeadersToRemove.Count);
        Assert.Equal("header3", httpRequestBuilder1.HeadersToRemove.First());
        Assert.Equal("header4", httpRequestBuilder1.HeadersToRemove.Last());

        var method2 = typeof(IHeaderDeclarativeTest).GetMethod(nameof(IHeaderDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder2, context2);

        Assert.NotNull(httpRequestBuilder2.Headers);
        Assert.Equal(3, httpRequestBuilder2.Headers.Count);
        Assert.Equal("value1", httpRequestBuilder2.Headers["header1"].First());
        Assert.Equal("value2", httpRequestBuilder2.Headers["header2"].First());
        Assert.Equal("value3", httpRequestBuilder2.Headers["header3"].First());

        var method3 = typeof(IHeaderDeclarativeTest).GetMethod(nameof(IHeaderDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder3, context3);

        Assert.NotNull(httpRequestBuilder3.Headers);
        Assert.Equal(3, httpRequestBuilder3.Headers.Count);
        Assert.Equal("value1", httpRequestBuilder3.Headers["header1"].First());
        Assert.Equal(["value2", "value21"], httpRequestBuilder3.Headers["header2"]);
        Assert.Equal("value4", httpRequestBuilder3.Headers["header4"].First());

        var method4 = typeof(IHeaderDeclarativeTest).GetMethod(nameof(IHeaderDeclarativeTest.Test4))!;
        var context4 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", 31, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder4, context4);

        Assert.NotNull(httpRequestBuilder4.Headers);
        Assert.Equal(8, httpRequestBuilder4.Headers.Count);
        Assert.Equal("value1", httpRequestBuilder4.Headers["header1"].First());
        Assert.Equal("value2", httpRequestBuilder4.Headers["header2"].First());
        Assert.Equal("value3", httpRequestBuilder4.Headers["header3"].First());
        Assert.Equal("1", httpRequestBuilder4.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder4.Headers["name"].First());
        Assert.Equal("furion", httpRequestBuilder4.Headers["myName"].First());
        Assert.Equal("31", httpRequestBuilder4.Headers["age"].First());
        Assert.Equal("GuangDong", httpRequestBuilder4.Headers["address"].First());

        var context5 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", null, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder5, context5);

        Assert.NotNull(httpRequestBuilder5.Headers);
        Assert.Equal(8, httpRequestBuilder5.Headers.Count);
        Assert.Equal("value1", httpRequestBuilder5.Headers["header1"].First());
        Assert.Equal("value2", httpRequestBuilder5.Headers["header2"].First());
        Assert.Equal("value3", httpRequestBuilder5.Headers["header3"].First());
        Assert.Equal("1", httpRequestBuilder5.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder5.Headers["name"].First());
        Assert.Equal("furion", httpRequestBuilder5.Headers["myName"].First());
        Assert.Equal("30", httpRequestBuilder5.Headers["age"].First());
        Assert.Equal("GuangDong", httpRequestBuilder5.Headers["address"].First());

        var method5 = typeof(IHeaderDeclarativeTest).GetMethod(nameof(IHeaderDeclarativeTest.Test5))!;
        var context6 = new HttpDeclarativeExtractorContext(method5, [new { id = 10, name = "furion" }],
            new HttpDeclarativeMethodMetadata(method5, typeof(IHeaderDeclarativeTest)));
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new HeaderDeclarativeExtractor().Extract(httpRequestBuilder6, context6);

        Assert.NotNull(httpRequestBuilder6.Headers);
        Assert.Equal(4, httpRequestBuilder6.Headers.Count);
        Assert.Equal("10", httpRequestBuilder6.Headers["id"].First());
        Assert.Equal("furion", httpRequestBuilder6.Headers["name"].First());
    }
}

[Header("header1", "value1")]
[Header("header2", "value2")]
[Header("header3")]
public interface IHeaderDeclarativeTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    [Header("header4")]
    Task Test1();

    [Post("http://localhost:5000")]
    [Header("header3", "value3")]
    Task Test2();

    [Post("http://localhost:5000")]
    [Header("header2", "value21")]
    [Header("header4", "value4")]
    Task Test3();

    [Post("http://localhost:5000")]
    [Header("header3", "value3")]
    Task Test4([Header] int id, [Header] [Header(AliasAs = "myName")] string name, [Header(Value = 30)] int? age,
        [Header("address")] string abc, [Header] CancellationToken cancellationToken);

    [Post("http://localhost:5000")]
    Task Test5([Header] object obj);
}