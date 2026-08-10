// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpBuilderTests
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
    public void Get_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Get((string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Get("http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Get((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Get(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Put_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Put((string)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Put("http://localhost");
        Assert.Equal(HttpMethod.Put, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Put((Uri)null!);
        Assert.Equal(HttpMethod.Put, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Put(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Put, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Post_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Post((string)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Post("http://localhost");
        Assert.Equal(HttpMethod.Post, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Post((Uri)null!);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Post(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Post, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Delete_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Delete((string)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Delete("http://localhost");
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Delete((Uri)null!);
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Delete(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Delete, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Head_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Head((string)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Head("http://localhost");
        Assert.Equal(HttpMethod.Head, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Head((Uri)null!);
        Assert.Equal(HttpMethod.Head, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Head(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Head, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Options_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Options((string)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Options("http://localhost");
        Assert.Equal(HttpMethod.Options, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Options((Uri)null!);
        Assert.Equal(HttpMethod.Options, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Options(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Options, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Trace_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Trace((string)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Trace("http://localhost");
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Trace((Uri)null!);
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Trace(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Trace, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Patch_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Patch((string)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Patch("http://localhost");
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder2.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Patch((Uri)null!);
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Patch(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Patch, httpRequestBuilder4.HttpMethod);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Query_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Query((string)null!);
        Assert.Equal("QUERY", httpRequestBuilder1.HttpMethod?.ToString());
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Query("http://localhost");
        Assert.Equal("QUERY", httpRequestBuilder2.HttpMethod?.ToString());
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri?.ToString());

        var httpRequestBuilder3 = HttpBuilder.Query((Uri)null!);
        Assert.Equal("QUERY", httpRequestBuilder3.HttpMethod?.ToString());
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Query(new Uri("http://localhost"));
        Assert.Equal("QUERY", httpRequestBuilder4.HttpMethod?.ToString());
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri?.ToString());
    }

    [Fact]
    public void Create_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpBuilder.Create((HttpMethod)null!, (Uri)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpBuilder.Create((string)null!, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpBuilder.Create(string.Empty, (string)null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpBuilder.Create(" ", (string)null!);
        });

        Assert.Throws<ArgumentNullException>(() =>
        {
            HttpBuilder.Create((string)null!, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpBuilder.Create(string.Empty, null!, null);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            HttpBuilder.Create(" ", null!, null);
        });
    }

    [Fact]
    public void Create_ReturnOK()
    {
        var httpRequestBuilder1 = HttpBuilder.Create(HttpMethod.Get, (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder1.HttpMethod);
        Assert.Null(httpRequestBuilder1.RequestUri);

        var httpRequestBuilder2 = HttpBuilder.Create(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder2.HttpMethod);
        Assert.NotNull(httpRequestBuilder2.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder2.RequestUri.ToString());

        var httpRequestBuilder3 = HttpBuilder.Create(HttpMethod.Get, (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder3.HttpMethod);
        Assert.Null(httpRequestBuilder3.RequestUri);

        var httpRequestBuilder4 = HttpBuilder.Create(HttpMethod.Get, "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder4.HttpMethod);
        Assert.NotNull(httpRequestBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder4.RequestUri.ToString());

        var httpRequestBuilder5 = HttpBuilder.Create("GET", (string)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder5.HttpMethod);
        Assert.Null(httpRequestBuilder5.RequestUri);

        var httpRequestBuilder6 = HttpBuilder.Create("GET", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder6.HttpMethod);
        Assert.NotNull(httpRequestBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder6.RequestUri.ToString());

        var httpRequestBuilder7 = HttpBuilder.Create("get", "http://localhost");
        Assert.Equal(HttpMethod.Get, httpRequestBuilder7.HttpMethod);
        Assert.NotNull(httpRequestBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder7.RequestUri.ToString());

        var httpRequestBuilder8 = HttpBuilder.Create("GET", (Uri)null!);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder8.HttpMethod);
        Assert.Null(httpRequestBuilder8.RequestUri);

        var httpRequestBuilder9 = HttpBuilder.Create("GET", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder9.HttpMethod);
        Assert.NotNull(httpRequestBuilder9.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder9.RequestUri.ToString());

        var httpRequestBuilder10 = HttpBuilder.Create("get", new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpRequestBuilder10.HttpMethod);
        Assert.NotNull(httpRequestBuilder10.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder10.RequestUri.ToString());

        var httpRequestBuilder11 = HttpBuilder.Create("Furion", (string)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder11.HttpMethod);

        var httpRequestBuilder12 = HttpBuilder.Create("Furion", (Uri)null!);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder12.HttpMethod);

        var httpRequestBuilder13 = HttpBuilder.Create(HttpMethod.Post, (string)null!, _ =>
        {
        });
        Assert.Equal(HttpMethod.Post, httpRequestBuilder13.HttpMethod);

        var httpRequestBuilder14 = HttpBuilder.Create("Furion", null!, null);
        Assert.Equal(HttpMethod.Parse("Furion"), httpRequestBuilder14.HttpMethod);

        var httpRequestBuilder15 = HttpBuilder.Create(HttpMethod.Post, (Uri)null!, null);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder15.HttpMethod);
    }

    [Fact]
    public void DownloadFile_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpBuilder.DownloadFile(null!, (Uri?)null));

    [Fact]
    public void DownloadFile_ReturnOK()
    {
        var httpFileDownloadBuilder =
            HttpBuilder.DownloadFile(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpFileDownloadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder.RequestUri.ToString());

        var httpFileDownloadBuilder2 = HttpBuilder.DownloadFile(HttpMethod.Post, (Uri?)null);
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder2.HttpMethod);
        Assert.Null(httpFileDownloadBuilder2.RequestUri);

        var httpFileDownloadBuilder3 = HttpBuilder.DownloadFile((string)null!);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder3.HttpMethod);
        Assert.Null(httpFileDownloadBuilder3.RequestUri);

        var httpFileDownloadBuilder4 = HttpBuilder.DownloadFile("http://localhost");
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder4.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder4.RequestUri.ToString());

        var httpFileDownloadBuilder5 = HttpBuilder.DownloadFile((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder5.HttpMethod);
        Assert.Null(httpFileDownloadBuilder5.RequestUri);

        var httpFileDownloadBuilder6 = HttpBuilder.DownloadFile(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpFileDownloadBuilder6.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder6.RequestUri.ToString());

        var httpFileDownloadBuilder7 = HttpBuilder.DownloadFile(HttpMethod.Post, "http://localhost");
        Assert.Equal(HttpMethod.Post, httpFileDownloadBuilder7.HttpMethod);
        Assert.NotNull(httpFileDownloadBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpFileDownloadBuilder7.RequestUri.ToString());
    }

    [Fact]
    public void UploadFile_ReturnOK()
    {
        var httpFileUploadBuilder =
            HttpBuilder.UploadFile(HttpMethod.Post, new Uri("http://localhost"), @"C:\Workspaces\furion.html");

        Assert.NotNull(httpFileUploadBuilder);
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder.FilePath);
        Assert.Equal("file", httpFileUploadBuilder.Name);

        var httpFileUploadBuilder2 = HttpBuilder.UploadFile(HttpMethod.Post, (Uri?)null, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder2.HttpMethod);
        Assert.Null(httpFileUploadBuilder2.RequestUri);

        var httpFileUploadBuilder3 = HttpBuilder.UploadFile((string)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder3.HttpMethod);
        Assert.Null(httpFileUploadBuilder3.RequestUri);

        var httpFileUploadBuilder4 = HttpBuilder.UploadFile("http://localhost", @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder4.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder4.RequestUri.ToString());

        var httpFileUploadBuilder5 = HttpBuilder.UploadFile((Uri)null!, @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder5.HttpMethod);
        Assert.Null(httpFileUploadBuilder5.RequestUri);

        var httpFileUploadBuilder6 =
            HttpBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder6.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder6.RequestUri.ToString());

        var httpFileUploadBuilder7 =
            HttpBuilder.UploadFile(new Uri("http://localhost"), @"C:\Workspaces\furion.html", "fileinfo");
        Assert.Equal(HttpMethod.Post, httpFileUploadBuilder7.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpFileUploadBuilder7.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\furion.html", httpFileUploadBuilder7.FilePath);
        Assert.Equal("fileinfo", httpFileUploadBuilder7.Name);

        var httpFileUploadBuilder8 =
            HttpBuilder.UploadFile(HttpMethod.Put, new Uri("http://localhost"), @"C:\Workspaces\furion.html",
                "fileinfo");
        Assert.Equal(HttpMethod.Put, httpFileUploadBuilder8.HttpMethod);
        Assert.NotNull(httpFileUploadBuilder8.RequestUri);
    }

    [Fact]
    public void ServerSentEvents_ReturnOK()
    {
        var httpServerSentEventsBuilder =
            HttpBuilder.ServerSentEvents(new Uri("http://localhost"), (_, _) => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder);
        Assert.NotNull(httpServerSentEventsBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder.RequestUri.ToString());

        var httpServerSentEventsBuilder2 =
            HttpBuilder.ServerSentEvents((Uri)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder2.RequestUri);

        var httpServerSentEventsBuilder3 =
            HttpBuilder.ServerSentEvents("http://localhost", (_, _) => Task.CompletedTask);
        Assert.NotNull(httpServerSentEventsBuilder3);
        Assert.NotNull(httpServerSentEventsBuilder3.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder3.RequestUri.ToString());

        var httpServerSentEventsBuilder4 =
            HttpBuilder.ServerSentEvents((string)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder4.RequestUri);

        var httpServerSentEventsBuilder5 =
            HttpBuilder.ServerSentEvents(HttpMethod.Post, (Uri?)null!, (_, _) => Task.CompletedTask);
        Assert.Null(httpServerSentEventsBuilder5.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder5.HttpMethod);

        var httpServerSentEventsBuilder6 =
            HttpBuilder.ServerSentEvents(new Uri("http://localhost"));
        Assert.NotNull(httpServerSentEventsBuilder6);
        Assert.NotNull(httpServerSentEventsBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder6.RequestUri.ToString());

        var httpServerSentEventsBuilder7 =
            HttpBuilder.ServerSentEvents((Uri)null!);
        Assert.Null(httpServerSentEventsBuilder7.RequestUri);

        var httpServerSentEventsBuilder8 =
            HttpBuilder.ServerSentEvents("http://localhost");
        Assert.NotNull(httpServerSentEventsBuilder8);
        Assert.NotNull(httpServerSentEventsBuilder8.RequestUri);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder8.RequestUri.ToString());

        var httpServerSentEventsBuilder9 =
            HttpBuilder.ServerSentEvents((string)null!);
        Assert.Null(httpServerSentEventsBuilder9.RequestUri);

        var httpServerSentEventsBuilder10 =
            HttpBuilder.ServerSentEvents(HttpMethod.Post, (Uri?)null!);
        Assert.Null(httpServerSentEventsBuilder10.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder10.HttpMethod);

        var httpServerSentEventsBuilder11 =
            HttpBuilder.ServerSentEvents(HttpMethod.Post, (string)null!);
        Assert.Null(httpServerSentEventsBuilder11.RequestUri);
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder11.HttpMethod);

        var httpServerSentEventsBuilder12 =
            HttpBuilder.ServerSentEvents(HttpMethod.Post, "http://localhost");
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder12.RequestUri?.ToString());
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder12.HttpMethod);

        var httpServerSentEventsBuilder13 =
            HttpBuilder.ServerSentEvents(HttpMethod.Post, "http://localhost", (_, _) => Task.CompletedTask);
        Assert.Equal("http://localhost/", httpServerSentEventsBuilder13.RequestUri?.ToString());
        Assert.Equal(HttpMethod.Post, httpServerSentEventsBuilder13.HttpMethod);
        Assert.NotNull(httpServerSentEventsBuilder13.OnMessage);
    }

    [Fact]
    public void StressTestHarness_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpBuilder.StressTestHarness(null!, (Uri?)null));

    [Fact]
    public void StressTestHarness_ReturnOK()
    {
        var httpStressTestHarnessBuilder =
            HttpBuilder.StressTestHarness(HttpMethod.Post, new Uri("http://localhost"));

        Assert.NotNull(httpStressTestHarnessBuilder);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder.RequestUri.ToString());

        var httpStressTestHarnessBuilder2 = HttpBuilder.StressTestHarness(HttpMethod.Post, (Uri?)null, 500);
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder2.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder2.RequestUri);
        Assert.Equal(500, httpStressTestHarnessBuilder2.NumberOfRequests);

        var httpStressTestHarnessBuilder3 = HttpBuilder.StressTestHarness((string)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder3.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder3.RequestUri);

        var httpStressTestHarnessBuilder4 = HttpBuilder.StressTestHarness("http://localhost");
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder4.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder4.RequestUri.ToString());

        var httpStressTestHarnessBuilder5 = HttpBuilder.StressTestHarness((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder5.HttpMethod);
        Assert.Null(httpStressTestHarnessBuilder5.RequestUri);

        var httpStressTestHarnessBuilder6 = HttpBuilder.StressTestHarness(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpStressTestHarnessBuilder6.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder6.RequestUri.ToString());

        var httpStressTestHarnessBuilder7 = HttpBuilder.StressTestHarness(HttpMethod.Post, "http://localhost");
        Assert.Equal(HttpMethod.Post, httpStressTestHarnessBuilder7.HttpMethod);
        Assert.NotNull(httpStressTestHarnessBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpStressTestHarnessBuilder7.RequestUri.ToString());
    }

    [Fact]
    public void LongPolling_ReturnOK()
    {
        var httpLongPollingBuilder =
            HttpBuilder.LongPolling(HttpMethod.Get, new Uri("http://localhost"), (_, _) => Task.CompletedTask);

        Assert.NotNull(httpLongPollingBuilder);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder.RequestUri.ToString());

        var httpLongPollingBuilder2 =
            HttpBuilder.LongPolling(HttpMethod.Get, (Uri?)null, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder2.HttpMethod);
        Assert.Null(httpLongPollingBuilder2.RequestUri);

        var httpLongPollingBuilder3 = HttpBuilder.LongPolling((string)null!, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder3.HttpMethod);
        Assert.Null(httpLongPollingBuilder3.RequestUri);

        var httpLongPollingBuilder4 = HttpBuilder.LongPolling("http://localhost", (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder4.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder4.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder4.RequestUri.ToString());

        var httpLongPollingBuilder5 = HttpBuilder.LongPolling((Uri)null!, (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder5.HttpMethod);
        Assert.Null(httpLongPollingBuilder5.RequestUri);

        var httpLongPollingBuilder6 =
            HttpBuilder.LongPolling(new Uri("http://localhost"), (_, _) => Task.CompletedTask);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder6.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder6.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder6.RequestUri.ToString());

        var httpLongPollingBuilder7 =
            HttpBuilder.LongPolling(HttpMethod.Get, new Uri("http://localhost"));

        Assert.NotNull(httpLongPollingBuilder7);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder7.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder7.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder7.RequestUri.ToString());

        var httpLongPollingBuilder8 =
            HttpBuilder.LongPolling(HttpMethod.Get, (Uri?)null);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder8.HttpMethod);
        Assert.Null(httpLongPollingBuilder8.RequestUri);

        var httpLongPollingBuilder9 = HttpBuilder.LongPolling((string)null!);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder9.HttpMethod);
        Assert.Null(httpLongPollingBuilder9.RequestUri);

        var httpLongPollingBuilder10 = HttpBuilder.LongPolling("http://localhost");
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder10.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder10.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder10.RequestUri.ToString());

        var httpLongPollingBuilder11 = HttpBuilder.LongPolling((Uri)null!);
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder11.HttpMethod);
        Assert.Null(httpLongPollingBuilder11.RequestUri);

        var httpLongPollingBuilder12 =
            HttpBuilder.LongPolling(new Uri("http://localhost"));
        Assert.Equal(HttpMethod.Get, httpLongPollingBuilder12.HttpMethod);
        Assert.NotNull(httpLongPollingBuilder12.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder12.RequestUri.ToString());

        var httpLongPollingBuilder13 =
            HttpBuilder.LongPolling(HttpMethod.Post, "http://localhost");
        Assert.NotNull(httpLongPollingBuilder13.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder13.RequestUri.ToString());
        Assert.Equal(HttpMethod.Post, httpLongPollingBuilder13.HttpMethod);

        var httpLongPollingBuilder14 =
            HttpBuilder.LongPolling(HttpMethod.Post, "http://localhost", (_, _) => Task.CompletedTask);
        Assert.NotNull(httpLongPollingBuilder14.RequestUri);
        Assert.Equal("http://localhost/", httpLongPollingBuilder14.RequestUri.ToString());
        Assert.NotNull(httpLongPollingBuilder14.OnDataReceived);
        Assert.Equal(HttpMethod.Post, httpLongPollingBuilder14.HttpMethod);
    }

    [Fact]
    public void Declarative_ReturnOK()
    {
        var method = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder = HttpBuilder.Declarative(method, []);

        Assert.NotNull(httpDeclarativeBuilder);
        Assert.Equal(method, httpDeclarativeBuilder.Method);
        Assert.Equal([], httpDeclarativeBuilder.Args);
        Assert.Equal(typeof(IHttpTest), httpDeclarativeBuilder.InterfaceType);

        var method2 = typeof(IHttpTest).GetMethod(nameof(IHttpTest.GetContent))!;
        var httpDeclarativeBuilder2 = HttpBuilder.Declarative(method2, [], typeof(IHttpTest));

        Assert.NotNull(httpDeclarativeBuilder2);
        Assert.Equal(method2, httpDeclarativeBuilder2.Method);
        Assert.Equal([], httpDeclarativeBuilder2.Args);
        Assert.Equal(typeof(IHttpTest), httpDeclarativeBuilder2.InterfaceType);
    }
}