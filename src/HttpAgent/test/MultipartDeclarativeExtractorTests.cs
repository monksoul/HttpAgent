// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class MultipartDeclarativeExtractorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(
            typeof(IHttpDeclarativeExtractor).IsAssignableFrom(
                typeof(MultipartDeclarativeExtractor)));

        var extractor = new MultipartDeclarativeExtractor();
        Assert.NotNull(extractor);
        Assert.Equal(3, extractor.Order);
    }

    [Fact]
    public void Extract_ReturnOK()
    {
        var method1 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test1))!;
        var context1 = new HttpDeclarativeExtractorContext(method1, []);
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder1, context1);
        Assert.Null(httpRequestBuilder1.MultipartFormDataBuilder);

        var method2 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2,
        [
            1, "furion", new { }, new MemoryStream(), Array.Empty<byte>(), new StringContent(""),
            MultipartFile.CreateFromByteArray([])
        ]);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder2, context2);
        Assert.NotNull(httpRequestBuilder2.MultipartFormDataBuilder);
        Assert.Equal(7, httpRequestBuilder2.MultipartFormDataBuilder._partContents.Count);
        Assert.Equal("id", httpRequestBuilder2.MultipartFormDataBuilder._partContents[0].Name);
        Assert.Equal("text/plain", httpRequestBuilder2.MultipartFormDataBuilder._partContents[0].ContentType);
        Assert.Equal("name", httpRequestBuilder2.MultipartFormDataBuilder._partContents[1].Name);
        Assert.Equal("text/plain", httpRequestBuilder2.MultipartFormDataBuilder._partContents[1].ContentType);
        Assert.Equal("obj", httpRequestBuilder2.MultipartFormDataBuilder._partContents[2].Name);
        Assert.Equal("text/plain", httpRequestBuilder2.MultipartFormDataBuilder._partContents[2].ContentType);
        Assert.Equal("stream", httpRequestBuilder2.MultipartFormDataBuilder._partContents[3].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder2.MultipartFormDataBuilder._partContents[3].ContentType);
        Assert.Equal("bytes", httpRequestBuilder2.MultipartFormDataBuilder._partContents[4].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder2.MultipartFormDataBuilder._partContents[4].ContentType);
        Assert.Equal("content", httpRequestBuilder2.MultipartFormDataBuilder._partContents[5].Name);
        Assert.Equal("text/plain", httpRequestBuilder2.MultipartFormDataBuilder._partContents[5].ContentType);
        Assert.Equal("file", httpRequestBuilder2.MultipartFormDataBuilder._partContents[6].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder2.MultipartFormDataBuilder._partContents[6].ContentType);
        Assert.Equal("--------------------", httpRequestBuilder2.MultipartFormDataBuilder.Boundary);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var base64String = Convert.ToBase64String(File.ReadAllBytes(filePath));
        const string url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";

        var method3 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3,
            ["furion", "none", filePath, base64String, url]);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder3, context3);
        Assert.NotNull(httpRequestBuilder3.MultipartFormDataBuilder);
        Assert.Equal(5, httpRequestBuilder3.MultipartFormDataBuilder._partContents.Count);
        Assert.Equal("name", httpRequestBuilder3.MultipartFormDataBuilder._partContents[0].Name);
        Assert.Equal("text/plain", httpRequestBuilder3.MultipartFormDataBuilder._partContents[0].ContentType);
        Assert.Equal("none", httpRequestBuilder3.MultipartFormDataBuilder._partContents[1].Name);
        Assert.Equal("text/plain", httpRequestBuilder3.MultipartFormDataBuilder._partContents[1].ContentType);
        Assert.Equal("filePath", httpRequestBuilder3.MultipartFormDataBuilder._partContents[2].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder3.MultipartFormDataBuilder._partContents[2].ContentType);
        Assert.Equal("base64String", httpRequestBuilder3.MultipartFormDataBuilder._partContents[3].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder3.MultipartFormDataBuilder._partContents[3].ContentType);
        Assert.Equal("remote", httpRequestBuilder3.MultipartFormDataBuilder._partContents[4].Name);
        Assert.Equal("application/octet-stream",
            httpRequestBuilder3.MultipartFormDataBuilder._partContents[4].ContentType);

        var method4 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test4))!;
        var context4 = new HttpDeclarativeExtractorContext(method4,
            [1, CancellationToken.None]);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder4, context4);
        Assert.NotNull(httpRequestBuilder4.MultipartFormDataBuilder);
        Assert.Single(httpRequestBuilder4.MultipartFormDataBuilder._partContents);
        Assert.Equal("id", httpRequestBuilder4.MultipartFormDataBuilder._partContents[0].Name);
        Assert.Equal("text/plain", httpRequestBuilder4.MultipartFormDataBuilder._partContents[0].ContentType);

        var method5 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test5))!;
        var context5 = new HttpDeclarativeExtractorContext(method5,
        [
            new HttpRemoteMultipartModel2
            {
                Id = 1,
                Name = "Furion",
                File = MultipartFile.CreateFromPath(Path.Combine(AppContext.BaseDirectory, "test.txt"), "File")
            }
        ]);
        var httpRequestBuilder5 = HttpRequestBuilder.Get("http://localhost");
        new MultipartDeclarativeExtractor().Extract(httpRequestBuilder5, context5);
        Assert.NotNull(httpRequestBuilder5.MultipartFormDataBuilder);
        Assert.Equal(3, httpRequestBuilder5.MultipartFormDataBuilder._partContents.Count);
        Assert.Equal("Id", httpRequestBuilder5.MultipartFormDataBuilder._partContents[0].Name);
        Assert.Equal("text/plain", httpRequestBuilder5.MultipartFormDataBuilder._partContents[0].ContentType);
        Assert.Equal("Name", httpRequestBuilder5.MultipartFormDataBuilder._partContents[1].Name);
        Assert.Equal("text/plain", httpRequestBuilder5.MultipartFormDataBuilder._partContents[1].ContentType);
        Assert.Equal("File", httpRequestBuilder5.MultipartFormDataBuilder._partContents[2].Name);
        Assert.Equal("text/plain", httpRequestBuilder5.MultipartFormDataBuilder._partContents[2].ContentType);
    }

    [Fact]
    public void SetMultipartFormData_ReturnOK()
    {
        var method1 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test1))!;
        var httpRequestBuilder1 = HttpRequestBuilder.Get("http://localhost");
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(httpRequestBuilder1);

        MultipartDeclarativeExtractor.SetMultipartFormData(method1, httpMultipartFormDataBuilder);
        Assert.NotNull(httpMultipartFormDataBuilder.Boundary);

        var method2 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test2))!;
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        var httpMultipartFormDataBuilder2 = new HttpMultipartFormDataBuilder(httpRequestBuilder2);

        MultipartDeclarativeExtractor.SetMultipartFormData(method2, httpMultipartFormDataBuilder2);
        Assert.NotNull(httpMultipartFormDataBuilder2.Boundary);
        Assert.StartsWith("--------------------", httpMultipartFormDataBuilder2.Boundary);
        Assert.True(httpMultipartFormDataBuilder2.OmitContentType);
    }

    [Fact]
    public void AddMultipart_ReturnOK()
    {
        var method2 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test2))!;
        var context2 = new HttpDeclarativeExtractorContext(method2,
        [
            1, "furion", new { }, new MemoryStream(), Array.Empty<byte>(), new StringContent(""),
            MultipartFile.CreateFromByteArray([])
        ]);
        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(httpRequestBuilder2);

        foreach (var (parameter, value) in context2.Parameters)
        {
            MultipartDeclarativeExtractor.AddMultipart(parameter, value, httpMultipartFormDataBuilder);
        }

        Assert.NotNull(httpMultipartFormDataBuilder);
        Assert.Equal(7, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("id", httpMultipartFormDataBuilder._partContents[0].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[0].ContentType);
        Assert.Equal("name", httpMultipartFormDataBuilder._partContents[1].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[1].ContentType);
        Assert.Equal("obj", httpMultipartFormDataBuilder._partContents[2].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[2].ContentType);
        Assert.Equal("stream", httpMultipartFormDataBuilder._partContents[3].Name);
        Assert.Equal("application/octet-stream",
            httpMultipartFormDataBuilder._partContents[3].ContentType);
        Assert.Equal("bytes", httpMultipartFormDataBuilder._partContents[4].Name);
        Assert.Equal("application/octet-stream",
            httpMultipartFormDataBuilder._partContents[4].ContentType);
        Assert.Equal("content", httpMultipartFormDataBuilder._partContents[5].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder._partContents[5].ContentType);
        Assert.Equal("file", httpMultipartFormDataBuilder._partContents[6].Name);
        Assert.Equal("application/octet-stream", httpMultipartFormDataBuilder._partContents[6].ContentType);


        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var base64String = Convert.ToBase64String(File.ReadAllBytes(filePath));
        const string url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";

        var method3 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test3))!;
        var context3 = new HttpDeclarativeExtractorContext(method3,
            ["furion", "none", filePath, base64String, url]);
        var httpRequestBuilder3 = HttpRequestBuilder.Get("http://localhost");
        var httpMultipartFormDataBuilder2 = new HttpMultipartFormDataBuilder(httpRequestBuilder3);

        foreach (var (parameter, value) in context3.Parameters)
        {
            MultipartDeclarativeExtractor.AddMultipart(parameter, value, httpMultipartFormDataBuilder2);
        }

        Assert.NotNull(httpMultipartFormDataBuilder2);
        Assert.Equal(5, httpMultipartFormDataBuilder2._partContents.Count);
        Assert.Equal("name", httpMultipartFormDataBuilder2._partContents[0].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder2._partContents[0].ContentType);
        Assert.Equal("none", httpMultipartFormDataBuilder2._partContents[1].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder2._partContents[1].ContentType);
        Assert.Equal("filePath", httpMultipartFormDataBuilder2._partContents[2].Name);
        Assert.Equal("application/octet-stream",
            httpMultipartFormDataBuilder2._partContents[2].ContentType);
        Assert.Equal("base64String", httpMultipartFormDataBuilder2._partContents[3].Name);
        Assert.Equal("application/octet-stream",
            httpMultipartFormDataBuilder2._partContents[3].ContentType);
        Assert.Equal("remote", httpMultipartFormDataBuilder2._partContents[4].Name);
        Assert.Equal("application/octet-stream",
            httpMultipartFormDataBuilder2._partContents[4].ContentType);

        var method4 =
            typeof(IMultipartDeclarativeExtractorTest).GetMethod(nameof(IMultipartDeclarativeExtractorTest.Test4))!;
        var context4 = new HttpDeclarativeExtractorContext(method4,
            [1, CancellationToken.None]);
        var httpRequestBuilder4 = HttpRequestBuilder.Get("http://localhost");
        var httpMultipartFormDataBuilder3 = new HttpMultipartFormDataBuilder(httpRequestBuilder4);

        foreach (var (parameter, value) in context4.Parameters)
        {
            MultipartDeclarativeExtractor.AddMultipart(parameter, value, httpMultipartFormDataBuilder3);
        }

        Assert.NotNull(httpMultipartFormDataBuilder3);
        Assert.Single(httpMultipartFormDataBuilder3._partContents);
        Assert.Equal("id", httpMultipartFormDataBuilder3._partContents[0].Name);
        Assert.Equal("text/plain", httpMultipartFormDataBuilder3._partContents[0].ContentType);
    }

    [Fact]
    public void AddFileFromSource_Invalid_Parameters()
    {
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => MultipartDeclarativeExtractor.AddFileFromSource(null!, "noneName",
            new MultipartAttribute("noneName"),
            httpMultipartFormDataBuilder, null));

        Assert.Throws<ArgumentException>(() => MultipartDeclarativeExtractor.AddFileFromSource(string.Empty,
            "noneName",
            new MultipartAttribute("noneName"),
            httpMultipartFormDataBuilder, null));

        Assert.Throws<ArgumentException>(() => MultipartDeclarativeExtractor.AddFileFromSource(" ", "noneName",
            new MultipartAttribute("noneName"),
            httpMultipartFormDataBuilder, null));
    }

    [Fact]
    public void AddFileFromSource_ReturnOK()
    {
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        MultipartDeclarativeExtractor.AddFileFromSource("none", "noneName", new MultipartAttribute("noneName"),
            httpMultipartFormDataBuilder, null);
        Assert.Empty(httpMultipartFormDataBuilder._partContents);

        MultipartDeclarativeExtractor.AddFileFromSource("none1", "noneName1",
            new MultipartAttribute("noneName1") { AsFileFrom = FileSourceType.None },
            httpMultipartFormDataBuilder, null);
        Assert.Empty(httpMultipartFormDataBuilder._partContents);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var base64String = Convert.ToBase64String(File.ReadAllBytes(filePath));
        const string url =
            "https://download2.huduntech.com/application/workspace/49/49d0cbe19a9bf7e54c1735b24fa41f27/Installer_%E8%BF%85%E6%8D%B7%E5%B1%8F%E5%B9%95%E5%BD%95%E5%83%8F%E5%B7%A5%E5%85%B7_1.7.9_123.exe";

        MultipartDeclarativeExtractor.AddFileFromSource(filePath, "file1",
            new MultipartAttribute("file1") { AsFileFrom = FileSourceType.Path },
            httpMultipartFormDataBuilder, null);
        Assert.Single(httpMultipartFormDataBuilder._partContents);
        Assert.Equal("file1", httpMultipartFormDataBuilder._partContents[0].Name);

        MultipartDeclarativeExtractor.AddFileFromSource(base64String, "file2",
            new MultipartAttribute("file2") { AsFileFrom = FileSourceType.Base64String },
            httpMultipartFormDataBuilder, null);
        Assert.Equal(2, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("file2", httpMultipartFormDataBuilder._partContents[1].Name);

        MultipartDeclarativeExtractor.AddFileFromSource(url, "file3",
            new MultipartAttribute("file3") { AsFileFrom = FileSourceType.Remote },
            httpMultipartFormDataBuilder, null);
        Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("file3", httpMultipartFormDataBuilder._partContents[2].Name);
    }

    [Fact]
    public void AddFormItemOrObject_ReturnOK()
    {
        var httpMultipartFormDataBuilder = new HttpMultipartFormDataBuilder(HttpRequestBuilder.Get("http://localhost"));

        MultipartDeclarativeExtractor.AddFormItemOrObject(null, "name", typeof(string), new MultipartAttribute("name"),
            httpMultipartFormDataBuilder, null);
        Assert.Single(httpMultipartFormDataBuilder._partContents);
        Assert.Equal("name", httpMultipartFormDataBuilder._partContents[0].Name);

        MultipartDeclarativeExtractor.AddFormItemOrObject("furion", "name1", typeof(string),
            new MultipartAttribute("name1"),
            httpMultipartFormDataBuilder, null);
        Assert.Equal(2, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("name1", httpMultipartFormDataBuilder._partContents[1].Name);

        MultipartDeclarativeExtractor.AddFormItemOrObject(new { }, "obj", typeof(object), new MultipartAttribute("obj"),
            httpMultipartFormDataBuilder, null);
        Assert.Equal(3, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("obj", httpMultipartFormDataBuilder._partContents[2].Name);

        MultipartDeclarativeExtractor.AddFormItemOrObject(new { id = 1, name = "furion" }, "obj1", typeof(object),
            new MultipartAttribute("obj1") { AsFormItem = false },
            httpMultipartFormDataBuilder, null);
        Assert.Equal(5, httpMultipartFormDataBuilder._partContents.Count);
        Assert.Equal("id", httpMultipartFormDataBuilder._partContents[3].Name);
        Assert.Equal("name", httpMultipartFormDataBuilder._partContents[4].Name);
    }
}

public interface IMultipartDeclarativeExtractorTest : IHttpDeclarative
{
    [Post("http://localhost:5000")]
    Task Test1();

    [Post("http://localhost:5000")]
    [MultipartForm("--------------------")]
    Task Test2([Multipart] int id, [Multipart] string name, [Multipart] object obj, [Multipart] Stream stream,
        [Multipart("bytes")] byte[] byteArray, [Multipart] StringContent content, [Multipart] MultipartFile file);

    [Post("http://localhost:5000")]
    Task Test3([Multipart] string name, [Multipart(AsFileFrom = FileSourceType.None)] string none,
        [Multipart(AsFileFrom = FileSourceType.Path)]
        string filePath,
        [Multipart(AsFileFrom = FileSourceType.Base64String)]
        string base64String, [Multipart(AsFileFrom = FileSourceType.Remote)] string remote);

    [Post("http://localhost:5000")]
    Task Test4([Multipart] int id, [Multipart] CancellationToken cancellationToken);

    [Post("http://localhost:5000")]
    Task Test5([MultipartObject] HttpRemoteMultipartModel2 data);
}