// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpFileUploadBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new HttpFileUploadBuilder(null!, null, null!, null!));
        Assert.Throws<ArgumentNullException>(() => new HttpFileUploadBuilder(HttpMethod.Post, null, null!, null!));
        Assert.Throws<ArgumentException>(() => new HttpFileUploadBuilder(HttpMethod.Post, null, string.Empty, null!));
        Assert.Throws<ArgumentException>(() => new HttpFileUploadBuilder(HttpMethod.Post, null, " ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", null!));
        Assert.Throws<ArgumentException>(() =>
            new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", string.Empty));
        Assert.Throws<ArgumentException>(() =>
            new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", " "));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", "file");
        Assert.Equal(HttpMethod.Post, builder.HttpMethod);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Equal(HttpMethod.Post, builder2.HttpMethod);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(@"C:\Workspaces\index.html", builder2.FilePath);
        Assert.Null(builder2.ContentType);
        Assert.Equal(TimeSpan.FromSeconds(1), builder2.ProgressInterval);
        Assert.Null(builder2.OnTransferStarted);
        Assert.Null(builder2.OnTransferCompleted);
        Assert.Null(builder2.OnTransferFailed);
        Assert.Null(builder2.OnProgressChanged);
        Assert.Null(builder2.FileTransferEventHandlerType);
        Assert.Null(builder2.AllowedFileExtensions);
        Assert.Null(builder2.MaxFileSizeInBytes);
        Assert.Null(builder2.FileName);
        Assert.Null(builder2._requestConfigure);

        var builder3 =
            new HttpFileUploadBuilder(HttpMethod.Post, null, @"C:\Workspaces\index.html", "file", "myindex.html");
        Assert.Equal("myindex.html", builder3.FileName);
    }

    [Fact]
    public void SetContentType_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() =>
        {
            builder.SetContentType(null!);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetContentType(string.Empty);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            builder.SetContentType(" ");
        });

        Assert.Throws<FormatException>(() =>
        {
            builder.SetContentType("unknown");
        });
    }

    [Fact]
    public void SetContentType_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetContentType("text/plain");
        Assert.Equal("text/plain", builder.ContentType);

        builder.SetContentType("text/html;charset=utf-8");
        Assert.Equal("text/html", builder.ContentType);

        builder.SetContentType("text/html; charset=unicode");
        Assert.Equal("text/html", builder.ContentType);
    }

    [Fact]
    public void SetAllowedFileExtensions_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() => builder.SetAllowedFileExtensions((string[])null!));
        Assert.Throws<ArgumentNullException>(() => builder.SetAllowedFileExtensions((string)null!));
        Assert.Throws<ArgumentException>(() => builder.SetAllowedFileExtensions(string.Empty));
        Assert.Throws<ArgumentException>(() => builder.SetAllowedFileExtensions(" "));
    }

    [Fact]
    public void SetAllowedFileExtensions_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        builder.SetAllowedFileExtensions([".html", ".exe"]);

        Assert.NotNull(builder.AllowedFileExtensions);
        Assert.Equal([".html", ".exe"], builder.AllowedFileExtensions);

        builder.SetAllowedFileExtensions(".jpg;.png;");
        Assert.NotNull(builder.AllowedFileExtensions);
        Assert.Equal([".jpg", ".png"], builder.AllowedFileExtensions);
    }

    [Fact]
    public void SetMaxFileSizeInBytes_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetMaxFileSizeInBytes(0));
        Assert.Equal("Max file size in bytes must be greater than zero. (Parameter 'maxFileSizeInBytes')",
            exception.Message);

        Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetMaxFileSizeInBytes(-1));
    }

    [Fact]
    public void SetMaxFileSizeInBytes_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        builder.SetMaxFileSizeInBytes(10);

        Assert.Equal(10, builder.MaxFileSizeInBytes);
    }

    [Fact]
    public void SetOnProgressChanged_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() => builder.SetOnProgressChanged(null!));
    }

    [Fact]
    public void SetOnProgressChanged_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Null(builder.OnProgressChanged);

        builder.SetOnProgressChanged(async _ =>
        {
            await Task.Delay(100);
        });

        Assert.NotNull(builder.OnProgressChanged);
    }

    [Fact]
    public void SetProgressInterval_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        var exception = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.Zero));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetProgressInterval(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Progress interval must be greater than 0. (Parameter 'progressInterval')", exception2.Message);
    }

    [Fact]
    public void SetProgressInterval_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetProgressInterval(TimeSpan.FromSeconds(1));

        Assert.Equal(TimeSpan.FromSeconds(1), builder.ProgressInterval);
    }

    [Fact]
    public void SetOnTransferStarted_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferStarted(null!));
    }

    [Fact]
    public void SetOnTransferStarted_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferStarted(() => { });

        Assert.NotNull(builder.OnTransferStarted);
    }

    [Fact]
    public void SetOnTransferCompleted_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferCompleted(null!));
    }

    [Fact]
    public void SetOnTransferCompleted_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferCompleted(_ => { });

        Assert.NotNull(builder.OnTransferCompleted);
    }

    [Fact]
    public void SetOnTransferFailed_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.SetOnTransferFailed(null!));
    }

    [Fact]
    public void SetOnTransferFailed_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetOnTransferFailed(_ => { });

        Assert.NotNull(builder.OnTransferFailed);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<ArgumentException>(() =>
            builder.SetEventHandler(typeof(NotImplementFileDownloadEventHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementFileDownloadEventHandler)}` type is not assignable from `{typeof(IHttpFileTransferEventHandler)}`. (Parameter 'fileTransferEventHandlerType')",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.SetEventHandler(typeof(CustomFileTransferEventHandler));

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder.FileTransferEventHandlerType);

        var builder2 = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder2.SetEventHandler<CustomFileTransferEventHandler>();

        Assert.Equal(typeof(CustomFileTransferEventHandler), builder2.FileTransferEventHandlerType);
    }

    [Fact]
    public void WithRequest_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Throws<ArgumentNullException>(() => builder.WithRequest(null!));
    }

    [Fact]
    public void WithRequest_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        Assert.Null(builder._requestConfigure);
        builder.WithRequest(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder._requestConfigure);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");
        builder.Profiler();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(false);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(_ => { });
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void EnsureLegalData_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => HttpFileUploadBuilder.EnsureLegalData(null!, null, null));
        Assert.Throws<ArgumentException>(() => HttpFileUploadBuilder.EnsureLegalData(string.Empty, null, null));
        Assert.Throws<ArgumentException>(() => HttpFileUploadBuilder.EnsureLegalData(" ", null, null));

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        Assert.Throws<InvalidOperationException>(() =>
            HttpFileUploadBuilder.EnsureLegalData(filePath, [".html"], null));

        Assert.Throws<InvalidOperationException>(() =>
            HttpFileUploadBuilder.EnsureLegalData(filePath, null, 10));

        Assert.Throws<InvalidOperationException>(() =>
            HttpFileUploadBuilder.EnsureLegalData(filePath, [".txt"], 10));
    }

    [Fact]
    public void EnsureLegalData_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        HttpFileUploadBuilder.EnsureLegalData(filePath, null, null);
        HttpFileUploadBuilder.EnsureLegalData(filePath, [".txt"], null);
        HttpFileUploadBuilder.EnsureLegalData(filePath, null, 100);
        HttpFileUploadBuilder.EnsureLegalData(filePath, [".txt"], 100);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            @"C:\Workspaces\index.html", "file");

        var httpRemoteOptions = new HttpRemoteOptions();

        Assert.Throws<ArgumentNullException>(() => builder.Build(null!, null!));
        Assert.Throws<ArgumentNullException>(() => builder.Build(httpRemoteOptions, null!));

        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();
        builder.SetAllowedFileExtensions(".txt");
        Assert.Throws<InvalidOperationException>(() => builder.Build(httpRemoteOptions, progressChannel));
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder = new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost"),
            filePath, "file").Profiler();

        var httpRemoteOptions = new HttpRemoteOptions();
        var progressChannel = Channel.CreateUnbounded<FileTransferProgress>();

        var httpRequestBuilder = httpFileUploadBuilder.Build(httpRemoteOptions, progressChannel);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder.HttpMethod);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.NotNull(httpRequestBuilder.MultipartFormDataBuilder);
        Assert.Single(httpRequestBuilder.MultipartFormDataBuilder._partContents);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        var item = httpRequestBuilder.MultipartFormDataBuilder._partContents[0];
        Assert.True(item.RawContent is ProgressFileStream);
        Assert.Equal("test.txt", item.FileName);
        Assert.Equal("text/plain", item.ContentType);
        Assert.Equal("file", item.Name);

        var httpRequestBuilder2 = httpFileUploadBuilder.SetEventHandler<CustomFileTransferEventHandler2>()
            .WithRequest(b => b.SetTimeout(100)).Build(httpRemoteOptions, progressChannel);

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }
}