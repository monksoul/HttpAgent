// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class PathDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(PathDeclarativeExtractor)));

        var extractor = new PathDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IPathDeclarativeTest).GetMethod(nameof(IPathDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, [],
            new HttpDeclarativeMethodMetadata(method1, typeof(IPathDeclarativeTest)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new PathDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.NotNull(httpRequestBuilder1.PathParameters);
        Assert.Equal(3, httpRequestBuilder1.PathParameters.Count);
        Assert.Equal(["path1", "path2", "path3"], httpRequestBuilder1.PathParameters.Keys);
        Assert.Null(httpRequestBuilder1.ObjectPathParameters);

        var method2 = typeof(IPathDeclarativeTest).GetMethod(nameof(IPathDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, [null!],
            new HttpDeclarativeMethodMetadata(method2, typeof(IPathDeclarativeTest)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new PathDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.PathParameters);
        Assert.Equal(2, httpRequestBuilder2.PathParameters.Count);
        Assert.Equal(["path1", "path2"], httpRequestBuilder2.PathParameters.Keys);
        Assert.Null(httpRequestBuilder2.ObjectPathParameters);

        var method3 = typeof(IPathDeclarativeTest).GetMethod(nameof(IPathDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3,
            [1, "furion", new[] { "广东省", "中山市" }, 30, new { id = 10, name = "furion" }, null, CancellationToken.None],
            new HttpDeclarativeMethodMetadata(method3, typeof(IPathDeclarativeTest)));
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new PathDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.PathParameters);
        Assert.Equal(6, httpRequestBuilder3.PathParameters.Count);
        Assert.NotNull(httpRequestBuilder3.ObjectPathParameters);
        Assert.Equal(2, httpRequestBuilder3.ObjectPathParameters.Count);
    }
}

[Path("path1", "value1")]
[Path("path2", "value2")]
public interface IPathDeclarativeTest : IHttpDeclarative
{
    [Path("path3", "value3")]
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2(Action<HttpRequestBuilder> builder);

    [Post("http://localhost:5000")]
    Task Test3(int id, string name, string[] address, int age, object? obj, object? obj2,
        CancellationToken cancellationToken);
}