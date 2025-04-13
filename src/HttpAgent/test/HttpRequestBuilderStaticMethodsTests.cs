// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderStaticMethodsTests
{
    [Fact]
    public void Get_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Get((string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Get("http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Get((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Get(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Put_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Put((string)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Put("http://localhost");
        Assert.Equal(HttpMethod.Put, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Put((Uri)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Put(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Put, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Post_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Post((string)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Post("http://localhost");
        Assert.Equal(HttpMethod.Post, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Post((Uri)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Post(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Post, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Delete_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Delete((string)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Delete("http://localhost");
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Delete((Uri)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Delete(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Head_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Head((string)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Head("http://localhost");
        Assert.Equal(HttpMethod.Head, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Head((Uri)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Head(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Head, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Options_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Options((string)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Options("http://localhost");
        Assert.Equal(HttpMethod.Options, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Options((Uri)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Options(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Options, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Trace_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Trace((string)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Trace("http://localhost");
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Trace((Uri)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Trace(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Patch_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Patch((string)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Patch("http://localhost");
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Patch((Uri)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Patch(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Create_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((HttpMethod)null!, (Uri)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((string)null!, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(string.Empty, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(" ", (string)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpRequestBuilder.Create((string)null!, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(string.Empty, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpRequestBuilder.Create(" ", null!, null);
        });
    }

    [Fact]
    public void Create_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Create(HttpMethod.Get, (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Create(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.HttpMethod);
        Assert.NotNull(httpRequestBuilder2.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Create(HttpMethod.Get, (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Create(HttpMethod.Get, "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.HttpMethod);
        Assert.NotNull(httpRequestBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri.ToString());

        var httpRequestBuilder5 = HttpRequestBuilder.Create("GET", (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder5.HttpMethod);
        Assert.Null(httpRequestBuilder5.RequestUri);

        var httpRequestBuilder6 = HttpRequestBuilder.Create("GET", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder6.HttpMethod);
        Assert.NotNull(httpRequestBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder6.RequestUri.ToString());

        var httpRequestBuilder7 = HttpRequestBuilder.Create("get", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder7.HttpMethod);
        Assert.NotNull(httpRequestBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder7.RequestUri.ToString());

        var httpRequestBuilder8 = HttpRequestBuilder.Create("GET", (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder8.HttpMethod);
        Assert.Null(httpRequestBuilder8.RequestUri);

        var httpRequestBuilder9 = HttpRequestBuilder.Create("GET", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder9.HttpMethod);
        Assert.NotNull(httpRequestBuilder9.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder9.RequestUri.ToString());

        var httpRequestBuilder10 = HttpRequestBuilder.Create("get", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder10.HttpMethod);
        Assert.NotNull(httpRequestBuilder10.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder10.RequestUri.ToString());

        var httpRequestBuilder11 = HttpRequestBuilder.Create("Furion", (string)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder11.HttpMethod);

        var httpRequestBuilder12 = HttpRequestBuilder.Create("Furion", (Uri)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder12.HttpMethod);

        var httpRequestBuilder13 = HttpRequestBuilder.Create(HttpMethod.Post, (string)null!, _ =>
        {
        });
        Assert.Equal(HttpMethod.Post, httpRequestBuilder13.HttpMethod);

        var httpRequestBuilder14 = HttpRequestBuilder.Create("Furion", null!, null);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder14.HttpMethod);

        var httpRequestBuilder15 = HttpRequestBuilder.Create(HttpMethod.Post, (Uri)null!, null);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder15.HttpMethod);
    }

    [Fact]
    public void DownloadFile_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.DownloadFile(null!, null, null, null));

    [Fact]
    public void DownloadFile_ReturnOK()
    {
        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(HttpMethod.Post, new Uri("http://localhost"), null);

        Assert.NotNull(httpFileDownloadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder.RequestUri.ToString());

        var httpFileDownloadBuilder2 = HttpRequestBuilder.DownloadFile(HttpMethod.Post, null, null);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder2.HttpMethod);
        Assert.Null(httpFileDownloadBuilder2.RequestUri);

        var httpFileDownloadBuilder3 = HttpRequestBuilder.DownloadFile((string)null!, null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder3.HttpMethod);
        Assert.Null(httpFileDownloadBuilder3.RequestUri);

        var httpFileDownloadBuilder4 = HttpRequestBuilder.DownloadFile("http://localhost", null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder4.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder4.RequestUri.ToString());

        var httpFileDownloadBuilder5 = HttpRequestBuilder.DownloadFile((Uri)null!, null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder5.HttpMethod);
        Assert.Null(httpFileDownloadBuilder5.RequestUri);

        var httpFileDownloadBuilder6 = HttpRequestBuilder.DownloadFile(new Uri("http://localhost"), null);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder6.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void UploadFile_ReturnOK()
    {
        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(HttpMethod.Post, new Uri("http://localhost"), @"C:\Workspaces\furion.html");

        Assert.NotNull(httpFileUploadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder.FilePath);
        Assert.Equal("file", httpFileUploadBuilder.Name);

        var httpFileUploadBuilder2 = HttpRequestBuilder.UploadFile(HttpMethod.Post, null, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder2.HttpMethod);
        Assert.Null(httpFileUploadBuilder2.RequestUri);

        var httpFileUploadBuilder3 = HttpRequestBuilder.UploadFile((string)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder3.HttpMethod);
        Assert.Null(httpFileUploadBuilder3.RequestUri);

        var httpFileUploadBuilder4 = HttpRequestBuilder.UploadFile("http://localhost", @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder4.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder4.RequestUri.ToString());

        var httpFileUploadBuilder5 = HttpRequestBuilder.UploadFile((Uri)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder5.HttpMethod);
        Assert.Null(httpFileUploadBuilder5.RequestUri);

        var httpFileUploadBuilder6 =
            HttpRequestBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder6.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder6.RequestUri.ToString());

        var httpFileUploadBuilder7 =
            HttpRequestBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html", "fileinfo");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder7.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder7.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder7.FilePath);
        Assert.Equal("fileinfo", httpFileUploadBuilder7.Name);
    }

    [Fact]
    public void ServerSentEvents_ReturnOK()
    {
        var httpServerSentEventsBuilder =
            HttpRequestBuilder.ServerSentEvents(new Uri("http://localhost"), (_, _) => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder);
        Assert.NotNull(httpServerSentEventsBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder.RequestUri.ToString());

        var httpServerSentEventsBuilder2 =
            HttpRequestBuilder.ServerSentEvents((Uri)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder2.RequestUri);

        var httpServerSentEventsBuilder3 =
            HttpRequestBuilder.ServerSentEvents("http://localhost", (_, _) => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder3);
        Assert.NotNull(httpServerSentEventsBuilder3.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder3.RequestUri.ToString());

        var httpServerSentEventsBuilder4 =
            HttpRequestBuilder.ServerSentEvents((string)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder4.RequestUri);

        var httpServerSentEventsBuilder5 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder5.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder5.HttpMethod);
    }

    [Fact]
    public void StressTestHarness_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.StressTestHarness(null!, null));

    [Fact]
    public void StressTestHarness_ReturnOK()
    {
        var httpStressTestHarnessBuilder =
            HttpRequestBuilder.StressTestHarness(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpStressTestHarnessBuilder);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder.RequestUri.ToString());

        var httpStressTestHarnessBuilder2 = HttpRequestBuilder.StressTestHarness(HttpMethod.Post, null, 500);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder2.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder2.RequestUri);
        Assert.Equal(500, httpStressTestHarnessBuilder2.NumberOfRequests);

        var httpStressTestHarnessBuilder3 = HttpRequestBuilder.StressTestHarness((string)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder3.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder3.RequestUri);

        var httpStressTestHarnessBuilder4 = HttpRequestBuilder.StressTestHarness("http://localhost");
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder4.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder4.RequestUri.ToString());

        var httpStressTestHarnessBuilder5 = HttpRequestBuilder.StressTestHarness((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder5.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder5.RequestUri);

        var httpStressTestHarnessBuilder6 = HttpRequestBuilder.StressTestHarness(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder6.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void LongPolling_ReturnOK()
    {
        var httpLongPollingBuilder =
            HttpRequestBuilder.LongPolling(HttpMethod.Get, new Uri("http://localhost"), (_, _) => Task.CompletedTask);

        Assert.NotNull(httpLongPollingBuilder);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder.RequestUri.ToString());

        var httpLongPollingBuilder2 =
            HttpRequestBuilder.LongPolling(HttpMethod.Get, null, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder2.HttpMethod);
        Assert.Null(httpLongPollingBuilder2.RequestUri);

        var httpLongPollingBuilder3 = HttpRequestBuilder.LongPolling((string)null!, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder3.HttpMethod);
        Assert.Null(httpLongPollingBuilder3.RequestUri);

        var httpLongPollingBuilder4 = HttpRequestBuilder.LongPolling("http://localhost", (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder4.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder4.RequestUri.ToString());

        var httpLongPollingBuilder5 = HttpRequestBuilder.LongPolling((Uri)null!, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder5.HttpMethod);
        Assert.Null(httpLongPollingBuilder5.RequestUri);

        var httpLongPollingBuilder6 =
            HttpRequestBuilder.LongPolling(new Uri("http://localhost"), (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder6.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder6.RequestUri.ToString());
    }

    [Fact]
    public void Declarative_ReturnOK()
    {
        var method = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder = HttpRequestBuilder.Declarative(method, []);

        Assert.NotNull(httpDeclarativeBuilder);
        Assert.Equal(method, httpDeclarativeBuilder.Method);
        Assert.Equal([], httpDeclarativeBuilder.Args);
    }

    [Fact]
    public void FromJson_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.FromJson(null!));
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson(string.Empty));
        Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson(" "));

        var exception = Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson("{}"));
        Assert.Equal("Missing required `method` in JSON.", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson("""{"method": null}"""));
        Assert.Equal("Missing required `method` in JSON.", exception2.Message);

        var exception3 = Assert.Throws<ArgumentException>(() => HttpRequestBuilder.FromJson("""{"method": "POST"}"""));
        Assert.Equal("Missing required `url` in JSON.", exception3.Message);

        var exception4 = Assert.Throws<InvalidOperationException>(() =>
            HttpRequestBuilder.FromJson("""{"method": "POST","url":null,"data":{}}"""));
        Assert.Equal("The `contentType` key is required when `data` is present.", exception4.Message);

        var exception5 = Assert.Throws<InvalidOperationException>(() =>
            HttpRequestBuilder.FromJson("""{"method": "POST","url":null,"data":{},"contentType":null}"""));
        Assert.Equal("The `contentType` key is required when `data` is present.", exception5.Message);

        var exception6 = Assert.Throws<InvalidOperationException>(() =>
            HttpRequestBuilder.FromJson("""{"method": "POST","url":null,"multipart":[]}"""));
        Assert.Equal("The node must be of type 'JsonObject'.", exception6.Message);
    }

    [Fact]
    public void FromJson_SetRequiredFields_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.FromJson("""
                                                             {
                                                                 "url": "/user",
                                                                 "method": "POST",
                                                             }
                                                             """);
        Assert.NotNull(httpRequestBuilder.HttpMethod);
        Assert.Equal("POST", httpRequestBuilder.HttpMethod.Method);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("/user", httpRequestBuilder.RequestUri.ToString());

        var httpRequestBuilder2 = HttpRequestBuilder.FromJson("""
                                                              {
                                                                  "url": null,
                                                                  "method": "POST",
                                                              }
                                                              """);
        Assert.Null(httpRequestBuilder2.RequestUri);
    }

    [Fact]
    public void FromJson_SetOptionalFields_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.FromJson("""
                                                             {
                                                                 "url": "/user",
                                                                 "method": "POST",
                                                                 "baseAddress": "https://furion.net",
                                                                 "headers": {
                                                                     "User-Agent": "HttpAgent/1.0.0",
                                                                     "Authorization": "Bearer xxxx"
                                                                 },
                                                                 "queries": {
                                                                     "id": 1,
                                                                     "name": "Furion"
                                                                 },
                                                                 "cookies": {
                                                                     "sessionId": "abcdefg123456",
                                                                     "userid": "monksoul"
                                                                 },
                                                                 "timeout": 20000,
                                                                 "client": "furion",
                                                                 "profiler": true
                                                             }
                                                             """);

        Assert.Equal("https://furion.net/", httpRequestBuilder.BaseAddress?.ToString());

        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Equal(2, httpRequestBuilder.Headers.Count);
        Assert.Equal("HttpAgent/1.0.0", httpRequestBuilder.Headers["User-Agent"].First());
        Assert.Equal("Bearer xxxx", httpRequestBuilder.Headers["Authorization"].First());

        Assert.NotNull(httpRequestBuilder.QueryParameters);
        Assert.Equal(2, httpRequestBuilder.QueryParameters.Count);
        Assert.Equal("1", httpRequestBuilder.QueryParameters["id"].First());
        Assert.Equal("Furion", httpRequestBuilder.QueryParameters["name"].First());

        Assert.NotNull(httpRequestBuilder.Cookies);
        Assert.Equal(2, httpRequestBuilder.Cookies.Count);
        Assert.Equal("abcdefg123456", httpRequestBuilder.Cookies["sessionId"]);
        Assert.Equal("monksoul", httpRequestBuilder.Cookies["userid"]);

        Assert.Equal(20000, httpRequestBuilder.Timeout!.Value.TotalMilliseconds);
        Assert.Equal("furion", httpRequestBuilder.HttpClientName);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void FromJson_SetContent_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.FromJson("""
                                                             {
                                                                 "url": "/user",
                                                                 "method": "POST",
                                                                 "contentType": "application/json",
                                                                 "encoding": "utf-8",
                                                                 "data": {
                                                                     "id": 1,
                                                                     "name": "百小僧",
                                                                     "age": 30
                                                                 }
                                                             }
                                                             """);


        Assert.Equal("application/json", httpRequestBuilder.ContentType);
        Assert.Equal("utf-8", httpRequestBuilder.ContentEncoding?.BodyName);
        Assert.Equal("{\"id\":1,\"name\":\"百小僧\",\"age\":30}", httpRequestBuilder.RawContent);
    }

    [Fact]
    public void FromJson_SetMultipartContent_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.FromJson("""
                                                             {
                                                                 "url": "/user",
                                                                 "method": "POST",
                                                                 "multipart": {
                                                                     "id": 1,
                                                                     "name": "furion",
                                                                 }
                                                             }
                                                             """);


        var multipartBuilder = httpRequestBuilder.MultipartFormDataBuilder;
        Assert.NotNull(multipartBuilder);

        Assert.Equal(2, multipartBuilder._partContents.Count);
        Assert.Equal("id", multipartBuilder._partContents[0].Name);
        Assert.Equal("text/plain", multipartBuilder._partContents[0].ContentType);
        Assert.Null(multipartBuilder._partContents[0].ContentEncoding);
        Assert.Equal("1", multipartBuilder._partContents[0].RawContent?.ToString());
        Assert.Equal("name", multipartBuilder._partContents[1].Name);
        Assert.Equal("text/plain", multipartBuilder._partContents[1].ContentType);
        Assert.Null(multipartBuilder._partContents[1].ContentEncoding);
        Assert.Equal("furion", multipartBuilder._partContents[1].RawContent?.ToString());
        Assert.NotNull(multipartBuilder._httpRequestBuilder.Disposables);
        Assert.Single(multipartBuilder._httpRequestBuilder.Disposables);
        Assert.True(multipartBuilder._httpRequestBuilder.Disposables.First() is JsonDocument);
    }

    [Fact]
    public void HandleJsonNode_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.HandleJsonNode(null!, null!, null!));

        var jsonObject = JsonNode.Parse("""{"id":1,"name":"furion"}""")!.AsObject();
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.HandleJsonNode(jsonObject, null!, null!));
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.HandleJsonNode(jsonObject, "id", null!));
    }

    [Fact]
    public void HandleJsonNode_ReturnOK()
    {
        var jsonObject = JsonNode.Parse("""{"id":1,"name":"furion"}""")!.AsObject();

        HttpRequestBuilder.HandleJsonNode(jsonObject, "id", node =>
        {
            Assert.Equal(1, node.GetValue<int>());
        });
    }
}