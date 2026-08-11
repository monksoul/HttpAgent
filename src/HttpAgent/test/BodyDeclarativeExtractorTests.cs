// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class BodyDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IHttpDeclarativeExtractor).IsAssignableFrom(typeof(BodyDeclarativeExtractor)));

        var extractor = new BodyDeclarativeExtractor();
        Assert.NotNull(extractor);
    }

    [Fact]
    public void Extract_Invalid_Parameters()
    {
        var method3 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test3))!;
        var context =
            new HttpDeclarativeParsingContext(method3, ["str1", "str2"],
                new HttpDeclarativeMetadata(method3, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder = HttpRequestBuilder.Get("http://localhost");

        Assert.Throws<InvalidOperationException>(() =>
            new BodyDeclarativeExtractor().Extract(httpRequestBuilder, context));
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test1))!;
        var context1 = new HttpDeclarativeParsingContext(method1, [],
            new HttpDeclarativeMetadata(method1, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.RawContent);
        Assert.Null(httpRequestBuilder1.ContentType);
        Assert.Null(httpRequestBuilder1.ContentEncoding?.WebName);

        var method2 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test2))!;
        var context2 =
            new HttpDeclarativeParsingContext(method2, ["str"],
                new HttpDeclarativeMetadata(method2, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.Equal("str", httpRequestBuilder2.RawContent);
        Assert.Null(httpRequestBuilder2.ContentType);
        Assert.Null(httpRequestBuilder2.ContentEncoding?.WebName);

        var method4 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test4))!;
        var context4 = new HttpDeclarativeParsingContext(method4, ["str", CancellationToken.None],
            new HttpDeclarativeMetadata(method4, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder4, context4);
        Assert.Equal("str", httpRequestBuilder4.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder4.ContentType);
        Assert.Null(httpRequestBuilder4.ContentEncoding?.WebName);

        var method5 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test5))!;
        var context5 =
            new HttpDeclarativeParsingContext(method5, ["str"],
                new HttpDeclarativeMetadata(method5, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder5, context5);
        Assert.Equal("str", httpRequestBuilder5.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder5.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder5.ContentEncoding?.WebName);

        var method6 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test6))!;
        var context6 =
            new HttpDeclarativeParsingContext(method6, ["str"],
                new HttpDeclarativeMetadata(method6, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder6 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder6, context6);
        Assert.Equal("str", httpRequestBuilder6.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder6.ContentType);
        Assert.Equal("utf-32", httpRequestBuilder6.ContentEncoding?.WebName);

        var method7 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test7))!;
        var context7 =
            new HttpDeclarativeParsingContext(method7, ["str"],
                new HttpDeclarativeMetadata(method7, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder7 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder7, context7);
        Assert.Equal("str", httpRequestBuilder7.RawContent);
        Assert.Equal("text/plain", httpRequestBuilder7.ContentType);
        Assert.Equal("utf-32", httpRequestBuilder7.ContentEncoding?.WebName);

        var method8 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test8))!;
        var context8 =
            new HttpDeclarativeParsingContext(method8, [new { }],
                new HttpDeclarativeMetadata(method8, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder8 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder8, context8);
        Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder8.ContentType);
        Assert.NotNull(httpRequestBuilder8.HttpContentProcessorProviders);
        Assert.Single(httpRequestBuilder8.HttpContentProcessorProviders);
        var stringContentForFormUrlEncodedContentProcessor =
            httpRequestBuilder8.HttpContentProcessorProviders[0].Invoke().First() as
                StringContentForFormUrlEncodedContentProcessor;
        Assert.NotNull(stringContentForFormUrlEncodedContentProcessor);
        Assert.True(stringContentForFormUrlEncodedContentProcessor.UrlEncode);

        var method9 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test9))!;
        var context9 =
            new HttpDeclarativeParsingContext(method9, ["Furion"],
                new HttpDeclarativeMetadata(method9, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder9 = HttpRequestBuilder.Post("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder9, context9);
        Assert.Equal("application/json", httpRequestBuilder9.ContentType);
        Assert.Equal("\"Furion\"", httpRequestBuilder9.RawContent);

        var method10 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test11))!;
        var context10 =
            new HttpDeclarativeParsingContext(method10, [new { }],
                new HttpDeclarativeMetadata(method10, typeof(IBodyDeclarativeTest)));
        var httpRequestBuilder10 = HttpRequestBuilder.Get("http://localhost");
        new BodyDeclarativeExtractor().Extract(httpRequestBuilder10, context10);
        Assert.Equal("application/x-www-form-urlencoded", httpRequestBuilder10.ContentType);
        Assert.NotNull(httpRequestBuilder10.HttpContentProcessorProviders);
        Assert.Single(httpRequestBuilder10.HttpContentProcessorProviders);
        var stringContentForFormUrlEncodedContentProcessor2 =
            httpRequestBuilder10.HttpContentProcessorProviders[0].Invoke().First() as
                StringContentForFormUrlEncodedContentProcessor;
        Assert.NotNull(stringContentForFormUrlEncodedContentProcessor2);
        Assert.False(stringContentForFormUrlEncodedContentProcessor2.UrlEncode);
    }

    [Fact]
    public void Extract_AsFile_ReturnOK()
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "hello world");

        try
        {
            var method12 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test12))!;
            var context12 = new HttpDeclarativeParsingContext(method12, [tempFile],
                new HttpDeclarativeMetadata(method12, typeof(IBodyDeclarativeTest)));
            var httpRequestBuilder12 = HttpRequestBuilder.Post("http://localhost");
            new BodyDeclarativeExtractor().Extract(httpRequestBuilder12, context12);

            Assert.NotNull(httpRequestBuilder12.RawContent);
            Assert.IsAssignableFrom<Stream>(httpRequestBuilder12.RawContent);
            Assert.Equal("text/plain", httpRequestBuilder12.ContentType);
            httpRequestBuilder12.ReleaseResources();

            var payload = new { name = "test" };
            var method13 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test13))!;
            var context13 = new HttpDeclarativeParsingContext(method13, [payload],
                new HttpDeclarativeMetadata(method13, typeof(IBodyDeclarativeTest)));
            var httpRequestBuilder13 = HttpRequestBuilder.Post("http://localhost");
            new BodyDeclarativeExtractor().Extract(httpRequestBuilder13, context13);
            Assert.Equal(payload, httpRequestBuilder13.RawContent);
            Assert.Null(httpRequestBuilder13.ContentType);

            var tempFile2 = Path.GetTempFileName();
            File.WriteAllText(tempFile2, "another");
            try
            {
                var method14 = typeof(IBodyDeclarativeTest).GetMethod(nameof(IBodyDeclarativeTest.Test14))!;
                var context14 = new HttpDeclarativeParsingContext(method14, [tempFile2],
                    new HttpDeclarativeMetadata(method14, typeof(IBodyDeclarativeTest)));
                var httpRequestBuilder14 = HttpRequestBuilder.Post("http://localhost");
                new BodyDeclarativeExtractor().Extract(httpRequestBuilder14, context14);
                Assert.NotNull(httpRequestBuilder14.RawContent);
                Assert.IsAssignableFrom<Stream>(httpRequestBuilder14.RawContent);
                httpRequestBuilder14.ReleaseResources();
            }
            finally
            {
                if (File.Exists(tempFile2))
                {
                    File.Delete(tempFile2);
                }
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}

public interface IBodyDeclarativeTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    Task Test2([Body] string body);

    [Post("http://localhost:5000")]
    Task Test3([Body] string body, [Body] string body2);

    [Post("http://localhost:5000")]
    Task Test4([Body("text/plain")] string body, [Body] CancellationToken cancellationToken);

    [Post("http://localhost:5000")]
    Task Test5([Body("text/plain", "utf-8")] string body);

    [Post("http://localhost:5000")]
    Task Test6([Body("text/plain; charset=utf-32")] string body);

    [Post("http://localhost:5000")]
    Task Test7([Body("text/plain; charset=utf-8", "utf-32")] string body);

    [Post("http://localhost:5000")]
    Task Test8([Body("application/x-www-form-urlencoded; charset=utf-8", UseStringContent = true)] object body);

    [Post("http://localhost:5000")]
    Task Test9([Body("application/json", RawString = true)] string body);

    [Post("http://localhost:5000")]
    Task Test10([Body(RawString = true)] string body);

    [Post("http://localhost:5000")]
    Task Test11([Body("application/x-www-form-urlencoded; charset=utf-8", UrlEncode = false)] object body);

    [Post("http://localhost:5000")]
    Task Test12([Body(AsFile = true, ContentType = "text/plain")] string filePath);

    [Post("http://localhost:5000")]
    Task Test13([Body(AsFile = true)] object notAString);

    [Post("http://localhost:5000")]
    Task Test14([Body(AsFile = true, RawString = true)] string filePath);
}