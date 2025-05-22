// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class PathSegmentDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(PathSegmentDeclarativeExtractor)));

        var extractor = new PathSegmentDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IPathSegmentDeclarativeTest).GetMethod(nameof(IPathSegmentDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new PathSegmentDeclarativeExtractor().Extract(httpRequestBuilder1, context1);

        Assert.NotNull(httpRequestBuilder1.PathSegments);
        Assert.Equal(2, httpRequestBuilder1.PathSegments.Count);
        Assert.Equal(["segment1", "segment2"], httpRequestBuilder1.PathSegments);
        Assert.NotNull(httpRequestBuilder1.PathSegmentsToRemove);
        Assert.Equal(2, httpRequestBuilder1.PathSegmentsToRemove.Count);
        Assert.Equal(["segment3", "segment4"], httpRequestBuilder1.PathSegmentsToRemove);

        var method2 = typeof(IPathSegmentDeclarativeTest).GetMethod(nameof(IPathSegmentDeclarativeTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2, []);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new PathSegmentDeclarativeExtractor().Extract(httpRequestBuilder2, context2);

        Assert.NotNull(httpRequestBuilder2.PathSegments);
        Assert.Equal(3, httpRequestBuilder2.PathSegments.Count);
        Assert.Equal(["segment1", "segment2", "segment3"], httpRequestBuilder2.PathSegments);
        Assert.Equal(["segment3"], httpRequestBuilder2.PathSegmentsToRemove);

        var method3 = typeof(IPathSegmentDeclarativeTest).GetMethod(nameof(IPathSegmentDeclarativeTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3, []);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new PathSegmentDeclarativeExtractor().Extract(httpRequestBuilder3, context3);

        Assert.NotNull(httpRequestBuilder3.PathSegments);
        Assert.Equal(4, httpRequestBuilder3.PathSegments.Count);
        Assert.Equal(["segment1", "segment2", "segment2", "segment4"], httpRequestBuilder3.PathSegments);

        var method4 = typeof(IPathSegmentDeclarativeTest).GetMethod(nameof(IPathSegmentDeclarativeTest.Test4))!;
        var context4 = new HttpDeclarativeExtractorContext(method4, [1, "furion", 31, "广东省", CancellationToken.None]);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new PathSegmentDeclarativeExtractor().Extract(httpRequestBuilder4, context4);

        Assert.NotNull(httpRequestBuilder4.PathSegments);
        Assert.Equal(8, httpRequestBuilder4.PathSegments.Count);
        Assert.Equal(["segment1", "segment2", "segment3", "1", "furion", "furion", "31", "广东省"],
            httpRequestBuilder4.PathSegments);

        var context5 = new HttpDeclarativeExtractorContext(method4, [1, "furion", null, "广东省", CancellationToken.None]);
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new PathSegmentDeclarativeExtractor().Extract(httpRequestBuilder5, context5);

        Assert.NotNull(httpRequestBuilder5.PathSegments);
        Assert.Equal(8, httpRequestBuilder5.PathSegments.Count);
        Assert.Equal(
            ["segment1", "segment2", "segment3", "1", "furion", "furion", "age", "广东省"],
            httpRequestBuilder5.PathSegments);
    }
}

[PathSegment("segment1")]
[PathSegment("segment2")]
[PathSegment("segment3", Remove = true)]
public interface IPathSegmentDeclarativeTest : IHttpDeclarative
{
    [Get("http://localhost:5000")]
    [PathSegment("segment4", Remove = true)]
    Task Test1();

    [Get("http://localhost:5000")]
    [PathSegment("segment3")]
    Task Test2();

    [Get("http://localhost:5000")]
    [PathSegment("segment2")]
    [PathSegment("segment4")]
    Task Test3();

    [Get("http://localhost:5000")]
    [PathSegment("segment3")]
    Task Test4([PathSegment] int id, [PathSegment] [PathSegment] string name, [PathSegment("age")] int? age,
        [PathSegment] string abc, [PathSegment] CancellationToken cancellationToken);
}