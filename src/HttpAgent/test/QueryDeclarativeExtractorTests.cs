// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class QueryDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(QueryDeclarativeExtractor)));

        var extractor = new QueryDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder1, context1);

        Assert.NotNull(httpRequestBuilder1.QueryParameters);
        Assert.Equal(2, httpRequestBuilder1.QueryParameters.Count);
        Assert.Equal("value1", httpRequestBuilder1.QueryParameters["query1"].First());
        Assert.Equal("value2", httpRequestBuilder1.QueryParameters["query2"].First());
        Assert.NotNull(httpRequestBuilder1.QueryParametersToRemove);
        Assert.Equal(2, httpRequestBuilder1.QueryParametersToRemove.Count);
        Assert.Equal("query3", httpRequestBuilder1.QueryParametersToRemove.First());
        Assert.Equal("query4", httpRequestBuilder1.QueryParametersToRemove.Last());

        var method2 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder2, context2);

        Assert.NotNull(httpRequestBuilder2.QueryParameters);
        Assert.Equal(3, httpRequestBuilder2.QueryParameters.Count);
        Assert.Equal("value1", httpRequestBuilder2.QueryParameters["query1"].First());
        Assert.Equal("value2", httpRequestBuilder2.QueryParameters["query2"].First());
        Assert.Equal("value3", httpRequestBuilder2.QueryParameters["query3"].First());

        var method3 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder3, context3);

        Assert.NotNull(httpRequestBuilder3.QueryParameters);
        Assert.Equal(3, httpRequestBuilder3.QueryParameters.Count);
        Assert.Equal("value1", httpRequestBuilder3.QueryParameters["query1"].First());
        Assert.Equal(["value2", "value21"], httpRequestBuilder3.QueryParameters["query2"]);
        Assert.Equal("value4", httpRequestBuilder3.QueryParameters["query4"].First());

        var method4 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test4))!;
        var context4 = new HttpDeclarativeExtractorContext(method4, [1, "furion", 31, "广东省", CancellationToken.None]);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder4, context4);

        Assert.NotNull(httpRequestBuilder4.QueryParameters);
        Assert.Equal(8, httpRequestBuilder4.QueryParameters.Count);
        Assert.Equal("value1", httpRequestBuilder4.QueryParameters["query1"].First());
        Assert.Equal("value2", httpRequestBuilder4.QueryParameters["query2"].First());
        Assert.Equal("value3", httpRequestBuilder4.QueryParameters["query3"].First());
        Assert.Equal(1, httpRequestBuilder4.QueryParameters["id"].First());
        Assert.Equal("furion", httpRequestBuilder4.QueryParameters["name"].First());
        Assert.Equal("furion", httpRequestBuilder4.QueryParameters["myName"].First());
        Assert.Equal(31, httpRequestBuilder4.QueryParameters["age"].First());
        Assert.Equal("广东省", httpRequestBuilder4.QueryParameters["address"].First());

        var context5 = new HttpDeclarativeExtractorContext(method4, [1, "furion", null, "广东省", CancellationToken.None]);
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder5, context5);

        Assert.NotNull(httpRequestBuilder5.QueryParameters);
        Assert.Equal(8, httpRequestBuilder5.QueryParameters.Count);
        Assert.Equal("value1", httpRequestBuilder5.QueryParameters["query1"].First());
        Assert.Equal("value2", httpRequestBuilder5.QueryParameters["query2"].First());
        Assert.Equal("value3", httpRequestBuilder5.QueryParameters["query3"].First());
        Assert.Equal(1, httpRequestBuilder5.QueryParameters["id"].First());
        Assert.Equal("furion", httpRequestBuilder5.QueryParameters["name"].First());
        Assert.Equal("furion", httpRequestBuilder5.QueryParameters["myName"].First());
        Assert.Equal(30, httpRequestBuilder5.QueryParameters["age"].First());
        Assert.Equal("广东省", httpRequestBuilder5.QueryParameters["address"].First());

        var method5 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test5))!;
        var context6 = new HttpDeclarativeExtractorContext(method5, [new { id = 10, name = "furion" }]);
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder6, context6);

        Assert.NotNull(httpRequestBuilder6.QueryParameters);
        Assert.Equal(4, httpRequestBuilder6.QueryParameters.Count);
        Assert.Equal(10, httpRequestBuilder6.QueryParameters["user.id"].First());
        Assert.Equal("furion", httpRequestBuilder6.QueryParameters["user.name"].First());

        var method6 = typeof(IQueryDeclarativeTest).GetMethod(nameof(IQueryDeclarativeTest.Test6))!;
        var context7 = new HttpDeclarativeExtractorContext(method6, [null, "abc"]);
        var httpRequestBuilder7 = HttpRequestBuilder.Get("http://localhost");
        new QueryDeclarativeExtractor().Extract(httpRequestBuilder7, context7);

        Assert.NotNull(httpRequestBuilder7.QueryParameters);
        Assert.Equal(3, httpRequestBuilder7.QueryParameters.Count);
        Assert.DoesNotContain(httpRequestBuilder7.QueryParameters, x => x.Key == "str1");
        Assert.Contains(httpRequestBuilder7.QueryParameters, x => x.Key == "str2");
    }
}

[Query("query1", "value1")]
[Query("query2", "value2")]
[Query("query3")]
public interface IQueryDeclarativeTest : IHttpDeclarative
{
    [Get("http://localhost:5000")]
    [Query("query4")]
    Task Test1();

    [Get("http://localhost:5000")]
    [Query("query3", "value3")]
    Task Test2();

    [Get("http://localhost:5000")]
    [Query("query2", "value21")]
    [Query("query4", "value4")]
    Task Test3();

    [Get("http://localhost:5000")]
    [Query("query3", "value3")]
    Task Test4([Query] int id, [Query] [Query(AliasAs = "myName")] string name, [Query(Value = 30)] int? age,
        [Query("address")] string abc, [Query] CancellationToken cancellationToken);

    [Get("http://localhost:5000")]
    Task Test5([Query(Prefix = "user")] object obj);

    [Get("http://localhost:5000")]
    Task Test6([Query(IgnoreNullValues = true)] string str1, [Query] string str2);
}