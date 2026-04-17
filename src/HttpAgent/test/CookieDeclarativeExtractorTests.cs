// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class CookieDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(CookieDeclarativeExtractor)));

        var extractor = new CookieDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(ICookieDeclarativeTest).GetMethod(nameof(ICookieDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder1, context1);

        Assert.NotNull(httpRequestBuilder1.Cookies);
        Assert.Equal(2, httpRequestBuilder1.Cookies.Count);
        Assert.Equal("value1", httpRequestBuilder1.Cookies["header1"]);
        Assert.Equal("value2", httpRequestBuilder1.Cookies["header2"]);
        Assert.NotNull(httpRequestBuilder1.CookiesToRemove);
        Assert.Equal(2, httpRequestBuilder1.CookiesToRemove.Count);
        Assert.Equal("header3", httpRequestBuilder1.CookiesToRemove.First());
        Assert.Equal("header4", httpRequestBuilder1.CookiesToRemove.Last());

        var method2 = typeof(ICookieDeclarativeTest).GetMethod(nameof(ICookieDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder2, context2);

        Assert.NotNull(httpRequestBuilder2.Cookies);
        Assert.Equal(3, httpRequestBuilder2.Cookies.Count);
        Assert.Equal("value1", httpRequestBuilder2.Cookies["header1"]);
        Assert.Equal("value2", httpRequestBuilder2.Cookies["header2"]);
        Assert.Equal("value3", httpRequestBuilder2.Cookies["header3"]);

        var method3 = typeof(ICookieDeclarativeTest).GetMethod(nameof(ICookieDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder3, context3);

        Assert.NotNull(httpRequestBuilder3.Cookies);
        Assert.Equal(3, httpRequestBuilder3.Cookies.Count);
        Assert.Equal("value1", httpRequestBuilder3.Cookies["header1"]);
        Assert.Equal("value21", httpRequestBuilder3.Cookies["header2"]);
        Assert.Equal("value4", httpRequestBuilder3.Cookies["header4"]);

        var method4 = typeof(ICookieDeclarativeTest).GetMethod(nameof(ICookieDeclarativeTest.Test4))!;
        var context4 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", 31, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder4, context4);

        Assert.NotNull(httpRequestBuilder4.Cookies);
        Assert.Equal(8, httpRequestBuilder4.Cookies.Count);
        Assert.Equal("value1", httpRequestBuilder4.Cookies["header1"]);
        Assert.Equal("value2", httpRequestBuilder4.Cookies["header2"]);
        Assert.Equal("value3", httpRequestBuilder4.Cookies["header3"]);
        Assert.Equal("1", httpRequestBuilder4.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder4.Cookies["name"]);
        Assert.Equal("furion", httpRequestBuilder4.Cookies["myName"]);
        Assert.Equal("31", httpRequestBuilder4.Cookies["age"]);
        Assert.Equal("GuangDong", httpRequestBuilder4.Cookies["address"]);

        var context5 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", null, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder5, context5);

        Assert.NotNull(httpRequestBuilder5.Cookies);
        Assert.Equal(8, httpRequestBuilder5.Cookies.Count);
        Assert.Equal("value1", httpRequestBuilder5.Cookies["header1"]);
        Assert.Equal("value2", httpRequestBuilder5.Cookies["header2"]);
        Assert.Equal("value3", httpRequestBuilder5.Cookies["header3"]);
        Assert.Equal("1", httpRequestBuilder5.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder5.Cookies["name"]);
        Assert.Equal("furion", httpRequestBuilder5.Cookies["myName"]);
        Assert.Equal("30", httpRequestBuilder5.Cookies["age"]);
        Assert.Equal("GuangDong", httpRequestBuilder5.Cookies["address"]);

        var method5 = typeof(ICookieDeclarativeTest).GetMethod(nameof(ICookieDeclarativeTest.Test5))!;
        var context6 = new HttpDeclarativeExtractorContext(method5, [new { id = 10, name = "furion" }],
            new HttpDeclarativeMethodMetadata(method5, typeof(ICookieDeclarativeTest)));
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new CookieDeclarativeExtractor().Extract(httpRequestBuilder6, context6);

        Assert.NotNull(httpRequestBuilder6.Cookies);
        Assert.Equal(4, httpRequestBuilder6.Cookies.Count);
        Assert.Equal("10", httpRequestBuilder6.Cookies["id"]);
        Assert.Equal("furion", httpRequestBuilder6.Cookies["name"]);
    }
}

[Cookie("header1", "value1")]
[Cookie("header2", "value2")]
[Cookie("header3")]
public interface ICookieDeclarativeTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    [Cookie("header4")]
    Task Test1();

    [Post("http://localhost:5000")]
    [Cookie("header3", "value3")]
    Task Test2();

    [Post("http://localhost:5000")]
    [Cookie("header2", "value21")]
    [Cookie("header4", "value4")]
    Task Test3();

    [Post("http://localhost:5000")]
    [Cookie("header3", "value3")]
    Task Test4([Cookie] int id, [Cookie] [Cookie(AliasAs = "myName")] string name, [Cookie(Value = 30)] int? age,
        [Cookie("address")] string abc, [Cookie] CancellationToken cancellationToken);

    [Post("http://localhost:5000")]
    Task Test5([Cookie] object obj);
}