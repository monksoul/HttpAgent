// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class PropertyDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(PropertyDeclarativeExtractor)));

        var extractor = new PropertyDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder1, context1);

        Assert.NotNull(httpRequestBuilder1.Properties);
        Assert.Equal(4, httpRequestBuilder1.Properties.Count);
        Assert.Equal("value1", httpRequestBuilder1.Properties["property1"]);
        Assert.Equal("value2", httpRequestBuilder1.Properties["property2"]);
        Assert.Null(httpRequestBuilder1.Properties["property3"]);
        Assert.Null(httpRequestBuilder1.Properties["property4"]);

        var method2 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [],
            new HttpDeclarativeMethodMetadata(method2, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder2, context2);

        Assert.NotNull(httpRequestBuilder2.Properties);
        Assert.Equal(3, httpRequestBuilder2.Properties.Count);
        Assert.Equal("value1", httpRequestBuilder2.Properties["property1"]);
        Assert.Equal("value2", httpRequestBuilder2.Properties["property2"]);
        Assert.Equal("value3", httpRequestBuilder2.Properties["property3"]);

        var method3 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, [],
            new HttpDeclarativeMethodMetadata(method3, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder3, context3);

        Assert.NotNull(httpRequestBuilder3.Properties);
        Assert.Equal(4, httpRequestBuilder3.Properties.Count);
        Assert.Equal("value1", httpRequestBuilder3.Properties["property1"]);
        Assert.Equal("value21", httpRequestBuilder3.Properties["property2"]);
        Assert.Null(httpRequestBuilder3.Properties["property3"]);
        Assert.Equal("value4", httpRequestBuilder3.Properties["property4"]);

        var method4 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test4))!;
        var context4 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", 31, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder4, context4);

        Assert.NotNull(httpRequestBuilder4.Properties);
        Assert.Equal(8, httpRequestBuilder4.Properties.Count);
        Assert.Equal("value1", httpRequestBuilder4.Properties["property1"]);
        Assert.Equal("value2", httpRequestBuilder4.Properties["property2"]);
        Assert.Equal("value3", httpRequestBuilder4.Properties["property3"]);
        Assert.Equal(1, httpRequestBuilder4.Properties["id"]);
        Assert.Equal("furion", httpRequestBuilder4.Properties["name"]);
        Assert.Equal("furion", httpRequestBuilder4.Properties["myName"]);
        Assert.Equal(31, httpRequestBuilder4.Properties["age"]);
        Assert.Equal("GuangDong", httpRequestBuilder4.Properties["address"]);

        var context5 =
            new HttpDeclarativeExtractorContext(method4, [1, "furion", null, "GuangDong", CancellationToken.None],
                new HttpDeclarativeMethodMetadata(method4, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder5, context5);

        Assert.NotNull(httpRequestBuilder5.Properties);
        Assert.Equal(8, httpRequestBuilder5.Properties.Count);
        Assert.Equal("value1", httpRequestBuilder5.Properties["property1"]);
        Assert.Equal("value2", httpRequestBuilder5.Properties["property2"]);
        Assert.Equal("value3", httpRequestBuilder5.Properties["property3"]);
        Assert.Equal(1, httpRequestBuilder5.Properties["id"]);
        Assert.Equal("furion", httpRequestBuilder5.Properties["name"]);
        Assert.Equal("furion", httpRequestBuilder5.Properties["myName"]);
        Assert.Equal(30, httpRequestBuilder5.Properties["age"]);
        Assert.Equal("GuangDong", httpRequestBuilder5.Properties["address"]);

        var method5 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test5))!;
        var context6 = new HttpDeclarativeExtractorContext(method5, [new { id = 10, name = "furion" }],
            new HttpDeclarativeMethodMetadata(method5, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder6, context6);

        Assert.NotNull(httpRequestBuilder6.Properties);
        Assert.Equal(4, httpRequestBuilder6.Properties.Count);
        Assert.NotNull(httpRequestBuilder6.Properties["obj"]);

        var method6 = typeof(IPropertyDeclarativeTest).GetMethod(nameof(IPropertyDeclarativeTest.Test6))!;
        var context7 = new HttpDeclarativeExtractorContext(method6, [new { id = 10, name = "furion" }],
            new HttpDeclarativeMethodMetadata(method6, typeof(IPropertyDeclarativeTest)));
        var httpRequestBuilder7 = HttpRequestBuilder.Get("http://localhost");
        new PropertyDeclarativeExtractor().Extract(httpRequestBuilder7, context7);

        Assert.NotNull(httpRequestBuilder7.Properties);
        Assert.Equal(5, httpRequestBuilder7.Properties.Count);
        Assert.Equal(10, httpRequestBuilder7.Properties["id"]);
        Assert.Equal("furion", httpRequestBuilder7.Properties["name"]);
    }
}

[Property("property1", "value1")]
[Property("property2", "value2")]
[Property("property3")]
public interface IPropertyDeclarativeTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    [Property("property4")]
    Task Test1();

    [Post("http://localhost:5000")]
    [Property("property3", "value3")]
    Task Test2();

    [Post("http://localhost:5000")]
    [Property("property2", "value21")]
    [Property("property4", "value4")]
    Task Test3();

    [Post("http://localhost:5000")]
    [Property("property3", "value3")]
    Task Test4([Property] int id, [Property] [Property(AliasAs = "myName")] string name,
        [Property(Value = 30)] int? age,
        [Property("address")] string abc, [Property] CancellationToken cancellationToken);

    [Post("http://localhost:5000")]
    Task Test5([Property] object obj);

    [Post("http://localhost:5000")]
    Task Test6([Property(AsItem = false)] object obj);
}