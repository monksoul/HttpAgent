// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderStaticMethodsTests
{
    [Fact]
    public void Setup_ReturnOK()
    {
        var httpRequestBuilder = HttpRequestBuilder.Setup;
        Assert.NotNull(httpRequestBuilder);
        Assert.Null(httpRequestBuilder.HttpMethod);
        Assert.Null(httpRequestBuilder.RequestUri);
    }

    [Fact]
    public void Operator_Action_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            Action<HttpRequestBuilder> _ = httpRequestBuilder;
        });

        Assert.Equal(
            "The 'Setup' builder must not have an HTTP method set. It is intended to be used as a configuration delegate for predicate-style methods (e.g., GetAsync, PostAsync).",
            exception.Message);
    }

    [Fact]
    public void Operator_Action_ReturnOK()
    {
        Action<HttpRequestBuilder> action = HttpRequestBuilder.Setup.UseETag();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        action(httpRequestBuilder);

        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri?.ToString());
        Assert.True(httpRequestBuilder.ETagEnabled);
    }

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
    public void Query_ReturnOK()
    {
        var httpRequestBuilder1 = HttpRequestBuilder.Query((string)null!);
        Assert.Equal("QUERY", httpRequestBuilder1.HttpMethod?.ToString());
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpRequestBuilder.Query("http://localhost");
        Assert.Equal("QUERY", httpRequestBuilder2.HttpMethod?.ToString());
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpRequestBuilder.Query((Uri)null!);
        Assert.Equal("QUERY", httpRequestBuilder3.HttpMethod?.ToString());
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpRequestBuilder.Query(new Uri("http://localhost"));
        Assert.Equal("QUERY", httpRequestBuilder4.HttpMethod?.ToString());
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
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.DownloadFile(null!, (Uri?)null));

    [Fact]
    public void DownloadFile_ReturnOK()
    {
        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpFileDownloadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder.RequestUri.ToString());

        var httpFileDownloadBuilder2 = HttpRequestBuilder.DownloadFile(HttpMethod.Post, (Uri?)null);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder2.HttpMethod);
        Assert.Null(httpFileDownloadBuilder2.RequestUri);

        var httpFileDownloadBuilder3 = HttpRequestBuilder.DownloadFile((string)null!);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder3.HttpMethod);
        Assert.Null(httpFileDownloadBuilder3.RequestUri);

        var httpFileDownloadBuilder4 = HttpRequestBuilder.DownloadFile("http://localhost");
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder4.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder4.RequestUri.ToString());

        var httpFileDownloadBuilder5 = HttpRequestBuilder.DownloadFile((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder5.HttpMethod);
        Assert.Null(httpFileDownloadBuilder5.RequestUri);

        var httpFileDownloadBuilder6 = HttpRequestBuilder.DownloadFile(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder6.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder6.RequestUri.ToString());

        var httpFileDownloadBuilder7 = HttpRequestBuilder.DownloadFile(HttpMethod.Post, "http://localhost");
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder7.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder7.RequestUri.ToString());
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

        var httpFileUploadBuilder2 =
            HttpRequestBuilder.UploadFile(HttpMethod.Post, (Uri?)null, @"C:\Workspaces\furion.html");
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

        var httpFileUploadBuilder8 =
            HttpRequestBuilder.UploadFile(HttpMethod.Put, new Uri("http://localhost"), @"C:\Workspaces\furion.html",
                "fileinfo");
        Assert.Equal(HttpMethod.Put, httpFileUploadBuilder8.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder8.RequestUri);
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
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, (Uri)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder5.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder5.HttpMethod);

        var httpServerSentEventsBuilder6 =
            HttpRequestBuilder.ServerSentEvents(new Uri("http://localhost"));
        Assert.NotNull(httpServerSentEventsBuilder6);
        Assert.NotNull(httpServerSentEventsBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder6.RequestUri.ToString());

        var httpServerSentEventsBuilder7 =
            HttpRequestBuilder.ServerSentEvents((Uri)null!);
        Assert.Null(httpServerSentEventsBuilder7.RequestUri);

        var httpServerSentEventsBuilder8 =
            HttpRequestBuilder.ServerSentEvents("http://localhost");
        Assert.NotNull(httpServerSentEventsBuilder8);
        Assert.NotNull(httpServerSentEventsBuilder8.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder8.RequestUri.ToString());

        var httpServerSentEventsBuilder9 =
            HttpRequestBuilder.ServerSentEvents((string)null!);
        Assert.Null(httpServerSentEventsBuilder9.RequestUri);

        var httpServerSentEventsBuilder10 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, (Uri)null!);
        Assert.Null(httpServerSentEventsBuilder10.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder10.HttpMethod);

        var httpServerSentEventsBuilder11 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, (string)null!);
        Assert.Null(httpServerSentEventsBuilder11.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder11.HttpMethod);

        var httpServerSentEventsBuilder12 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, "http://localhost");
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder12.RequestUri?.ToString());
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder12.HttpMethod);

        var httpServerSentEventsBuilder13 =
            HttpRequestBuilder.ServerSentEvents(HttpMethod.Post, "http://localhost", (_, _) => Task.CompletedTask);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder13.RequestUri?.ToString());
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder13.HttpMethod);
        Assert.NotNull(httpServerSentEventsBuilder13.OnMessage);
    }

    [Fact]
    public void StressTestHarness_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRequestBuilder.StressTestHarness(null!, (Uri?)null));

    [Fact]
    public void StressTestHarness_ReturnOK()
    {
        var httpStressTestHarnessBuilder =
            HttpRequestBuilder.StressTestHarness(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpStressTestHarnessBuilder);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder.RequestUri.ToString());

        var httpStressTestHarnessBuilder2 = HttpRequestBuilder.StressTestHarness(HttpMethod.Post, (Uri?)null, 500);
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

        var httpStressTestHarnessBuilder7 = HttpRequestBuilder.StressTestHarness(HttpMethod.Post, "http://localhost");
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder7.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder7.RequestUri.ToString());
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
            HttpRequestBuilder.LongPolling(HttpMethod.Get, (Uri?)null, (_, _) => Task.CompletedTask);
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

        var httpLongPollingBuilder7 =
            HttpRequestBuilder.LongPolling(HttpMethod.Get, new Uri("http://localhost"));

        Assert.NotNull(httpLongPollingBuilder7);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder7.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder7.RequestUri.ToString());

        var httpLongPollingBuilder8 =
            HttpRequestBuilder.LongPolling(HttpMethod.Get, (Uri?)null);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder8.HttpMethod);
        Assert.Null(httpLongPollingBuilder8.RequestUri);

        var httpLongPollingBuilder9 = HttpRequestBuilder.LongPolling((string)null!);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder9.HttpMethod);
        Assert.Null(httpLongPollingBuilder9.RequestUri);

        var httpLongPollingBuilder10 = HttpRequestBuilder.LongPolling("http://localhost");
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder10.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder10.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder10.RequestUri.ToString());

        var httpLongPollingBuilder11 = HttpRequestBuilder.LongPolling((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder11.HttpMethod);
        Assert.Null(httpLongPollingBuilder11.RequestUri);

        var httpLongPollingBuilder12 =
            HttpRequestBuilder.LongPolling(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder12.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder12.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder12.RequestUri.ToString());

        var httpLongPollingBuilder13 =
            HttpRequestBuilder.LongPolling(HttpMethod.Post, "http://localhost");
        Assert.NotNull(httpLongPollingBuilder13.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder13.RequestUri.ToString());
        Assert.Equal(HttpMethod.Post, httpLongPollingBuilder13.HttpMethod);

        var httpLongPollingBuilder14 =
            HttpRequestBuilder.LongPolling(HttpMethod.Post, "http://localhost", (_, _) => Task.CompletedTask);
        Assert.NotNull(httpLongPollingBuilder14.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder14.RequestUri.ToString());
        Assert.NotNull(httpLongPollingBuilder14.OnDataReceived);
        Assert.Equal(HttpMethod.Post, httpLongPollingBuilder14.HttpMethod);
    }

    [Fact]
    public void Declarative_ReturnOK()
    {
        var method = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder = HttpRequestBuilder.Declarative(method, []);

        Assert.NotNull(httpDeclarativeBuilder);
        Assert.Equal(method, httpDeclarativeBuilder.Method);
        Assert.Equal([], httpDeclarativeBuilder.Args);
        Assert.Equal(typeof(IHttpTest), httpDeclarativeBuilder.InterfaceType);

        var method2 = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder2 = HttpRequestBuilder.Declarative(method2, [], typeof(IHttpTest));

        Assert.NotNull(httpDeclarativeBuilder2);
        Assert.Equal(method2, httpDeclarativeBuilder2.Method);
        Assert.Equal([], httpDeclarativeBuilder2.Args);
        Assert.Equal(typeof(IHttpTest), httpDeclarativeBuilder2.InterfaceType);
    }
}