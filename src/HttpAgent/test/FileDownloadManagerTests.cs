// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileDownloadManagerTests(ITestOutputHelper output)
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new FileDownloadManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new FileDownloadManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var fileDownloadManager = new FileDownloadManager(httpRemoteService,
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost:5000")).SetDestinationPath(
                @"C:\Workspaces"));

        Assert.NotNull(fileDownloadManager._httpFileDownloadBuilder);
        Assert.NotNull(fileDownloadManager._httpRemoteService);
        Assert.NotNull(fileDownloadManager._progressChannel);
        Assert.Equal("UnboundedChannel`1", fileDownloadManager._progressChannel.GetType().Name);
        Assert.NotNull(fileDownloadManager.RequestBuilder);
        Assert.Null(fileDownloadManager.FileTransferEventHandler);
        Assert.Equal(0, fileDownloadManager._totalBytesReceived);

        var fileDownloadManager2 = new FileDownloadManager(httpRemoteService,
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("http://localhost:5000")).SetDestinationPath(
                @"C:\Workspaces").WithRequest(builder => builder.SetTimeout(100)));
        Assert.NotNull(fileDownloadManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), fileDownloadManager2.RequestBuilder.Timeout);

        serviceProvider.Dispose();
    }

    [Fact]
    public void GetFileName_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            httpRemoteService.Send(fileDownloadManager.RequestBuilder, HttpCompletionOption.ResponseHeadersRead);

        Assert.Throws<ArgumentException>(() => fileDownloadManager.GetFileName(httpResponseMessage!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void GetFileName_IfInDestinationPath_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            httpRemoteService.Send(fileDownloadManager.RequestBuilder, HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal("index.html", fileDownloadManager.GetFileName(httpResponseMessage!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void GetFileName_IfInUri_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net/index.html")).SetDestinationPath(
                @"C:\Workspaces\");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            httpRemoteService.Send(fileDownloadManager.RequestBuilder, HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal("index.html", fileDownloadManager.GetFileName(httpResponseMessage!));

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task GetFileNameAsync_IfInContentDisposition_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            context.Response.Headers.Append("Content-Disposition", "attachment; filename=index.html");
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                @"C:\Workspaces\");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            await httpRemoteService.SendAsync(fileDownloadManager.RequestBuilder,
                HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal("index.html", fileDownloadManager.GetFileName(httpResponseMessage!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void ShouldContinueWithDownload_Invalid_Parameters()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(filePath);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            httpRemoteService.Send(fileDownloadManager.RequestBuilder, HttpCompletionOption.ResponseHeadersRead);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            fileDownloadManager.ShouldContinueWithDownload(httpResponseMessage!, out _));

        Assert.Equal($"The destination path `{filePath}` already exists.", exception.Message);

        serviceProvider.Dispose();
    }

    [Fact]
    public void ShouldContinueWithDownload_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage =
            httpRemoteService.Send(fileDownloadManager.RequestBuilder, HttpCompletionOption.ResponseHeadersRead);

        Assert.True(fileDownloadManager.ShouldContinueWithDownload(httpResponseMessage!, out var destinationPath));
        Assert.Equal(@"C:\Workspaces\index.html", destinationPath);

        var fileDownloadManager2 = new FileDownloadManager(httpRemoteService,
            httpFileDownloadBuilder.SetFileExistsBehavior(FileExistsBehavior.CreateNew));
        Assert.True(fileDownloadManager2.ShouldContinueWithDownload(httpResponseMessage!, out var destinationPath2));
        Assert.Equal(@"C:\Workspaces\index.html", destinationPath2);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileDownloadManager3 = new FileDownloadManager(httpRemoteService,
            httpFileDownloadBuilder.SetFileExistsBehavior(FileExistsBehavior.Skip).SetDestinationPath(filePath));
        Assert.False(fileDownloadManager3.ShouldContinueWithDownload(httpResponseMessage!, out var destinationPath3));
        Assert.Equal(filePath, destinationPath3);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task ReportProgressAsync_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html").SetOnProgressChanged(async _ =>
            {
                i += 1;
                await Task.CompletedTask;
            }).SetProgressInterval(TimeSpan.FromMilliseconds(50));
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        using var progressCancellationTokenSource = new CancellationTokenSource();
        var reportProgressTask =
            fileDownloadManager.ReportProgressAsync(progressCancellationTokenSource.Token, CancellationToken.None);

        for (var j = 0; j < 3; j++)
        {
            await fileDownloadManager._progressChannel.Writer.WriteAsync(
                new FileTransferProgress(@"C:\Workspaces\index.html", -1), progressCancellationTokenSource.Token);
        }

        await Task.Delay(200, progressCancellationTokenSource.Token);

        fileDownloadManager._progressChannel.Writer.Complete();

        await progressCancellationTokenSource.CancelAsync();
        await reportProgressTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ReportProgressAsync_WithSetOnProgressChanged_Exception_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html").SetOnProgressChanged(async _ =>
            {
                i += 1;
                throw new Exception("Error");
#pragma warning disable CS0162 // 检测到不可到达的代码
                await Task.CompletedTask;
#pragma warning restore CS0162 // 检测到不可到达的代码
            }).SetProgressInterval(TimeSpan.FromMilliseconds(50));
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        using var progressCancellationTokenSource = new CancellationTokenSource();
        var reportProgressTask =
            fileDownloadManager.ReportProgressAsync(progressCancellationTokenSource.Token, CancellationToken.None);

        for (var j = 0; j < 3; j++)
        {
            await fileDownloadManager._progressChannel.Writer.WriteAsync(
                new FileTransferProgress(@"C:\Workspaces\index.html", -1), progressCancellationTokenSource.Token);
        }

        await Task.Delay(200, progressCancellationTokenSource.Token);

        fileDownloadManager._progressChannel.Writer.Complete();

        await progressCancellationTokenSource.CancelAsync();
        await reportProgressTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void HandleTransferStarted_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html").SetOnTransferStarted(() => throw new Exception("出错了"));
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        fileDownloadManager.HandleTransferStarted();

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleTransferCompleted_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html").SetOnTransferCompleted(_ => throw new Exception("出错了"));
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        fileDownloadManager.HandleTransferCompleted(100);

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleTransferFailed_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html").SetOnTransferFailed(_ => throw new Exception("出错了"));
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        fileDownloadManager.HandleTransferFailed(new Exception("出错了"));

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleFileExistAndSkip_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                Path.Combine(AppContext.BaseDirectory, "test.txt")).SetOnFileExistAndSkip(() =>
            {
                i++;
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        fileDownloadManager.HandleFileExistAndSkip();
        Assert.Equal(1, i);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task HandleProgressChangedAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fileDownloadManager.HandleProgressChangedAsync(null!));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HandleProgressChangedAsync_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var fileTransferProgress =
            new FileTransferProgress(@"C:\Workspaces\index.html", -1);
        await fileDownloadManager.HandleProgressChangedAsync(fileTransferProgress);

        var i = 0;
        httpFileDownloadBuilder.SetOnProgressChanged(async _ =>
        {
            i++;
            await Task.CompletedTask;
        });

        var fileDownloadManager2 = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);
        await fileDownloadManager2.HandleProgressChangedAsync(fileTransferProgress);

        Assert.Equal(1, i);
        Assert.Equal(0, customFileTransferEventHandler.counter2);

        httpFileDownloadBuilder.SetEventHandler<CustomFileTransferEventHandler>();
        var fileDownloadManager3 = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);
        await fileDownloadManager3.HandleProgressChangedAsync(fileTransferProgress);

        Assert.Equal(1, customFileTransferEventHandler.counter2);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void MoveTempFileToDestinationPath_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(null!, null!, null!));

        var tempDestinationPath = Path.GetTempFileName();
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "tests", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var directoryPath = Path.GetDirectoryName(destinationPath)!;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
        var info = new UTF8Encoding(true).GetBytes("Hello World!");
        fileStream.Write(info, 0, info.Length);
        fileStream.Close();

        Assert.Throws<ArgumentNullException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, null!, null!));
        Assert.Throws<ArgumentException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, string.Empty, null!));
        Assert.Throws<ArgumentException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, " ", null!));

        Assert.Throws<ArgumentNullException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, tempDestinationPath, null!));
        Assert.Throws<ArgumentException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, tempDestinationPath, string.Empty));
        Assert.Throws<ArgumentException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, tempDestinationPath, " "));

        var exception = Assert.Throws<FileNotFoundException>(() =>
            FileDownloadManager.MoveTempFileToDestinationPath(fileStream, @"C:/unittest/temp/not-found/test.txt",
                destinationPath));
        Assert.Equal("The temp destination path `C:/unittest/temp/not-found/test.txt` does not exist.",
            exception.Message);
    }

    [Fact]
    public void MoveTempFileToDestinationPath_ReturnOK()
    {
        var tempDestinationPath = Path.GetTempFileName();
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "tests", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var directoryPath = Path.GetDirectoryName(destinationPath)!;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using var fileStream = new FileStream(tempDestinationPath, FileMode.Create, FileAccess.Write);
        var info = new UTF8Encoding(true).GetBytes("Hello World!");
        fileStream.Write(info, 0, info.Length);
        fileStream.Close();

        FileDownloadManager.MoveTempFileToDestinationPath(fileStream, tempDestinationPath, destinationPath);

        Assert.True(File.Exists(destinationPath));
        File.Delete(destinationPath);
    }

    [Fact]
    public async Task Start_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test2.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        fileDownloadManager.Start();

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithCancellationToken_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnProgressChanged(async _ =>
            {
                await Task.CompletedTask;
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            fileDownloadManager.Start(cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Assert.True(e is OperationCanceledException);
        }

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithOnProgressChanged_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnProgressChanged(async _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        fileDownloadManager.Start();

        Assert.Equal(1, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_Filter_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test5.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        fileDownloadManager.Start();

        Assert.Equal(2, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_EventHandler_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            }).SetEventHandler<CustomFileTransferEventHandler>();

        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        fileDownloadManager.Start();

        Assert.Equal(2, i);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithTransferFailed_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
            await context.Response.WriteAsync("Hello World!");
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            }).SetOnTransferFailed(e =>
            {
                i += 1;
                output.WriteLine($"下载出错 {e.Message}");
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        Assert.Throws<HttpRequestException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            fileDownloadManager.Start();
        });
        Assert.Equal(2, i);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await fileDownloadManager.StartAsync();

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithCancellationToken_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnProgressChanged(async _ =>
            {
                await Task.CompletedTask;
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await fileDownloadManager.StartAsync(cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithOnProgressChanged_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnProgressChanged(async _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await fileDownloadManager.StartAsync();

        Assert.Equal(1, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_Filter_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await fileDownloadManager.StartAsync();

        Assert.Equal(2, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_EventHandler_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            }).SetEventHandler<CustomFileTransferEventHandler>();

        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await fileDownloadManager.StartAsync();

        Assert.Equal(2, i);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithTransferFailed_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test.txt");
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
            await context.Response.WriteAsync("Hello World!");
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test")).SetDestinationPath(
                destinationPath).SetOnTransferStarted(() =>
            {
                i += 1;
                output.WriteLine("准备下载...");
            }).SetOnTransferCompleted(elapsed =>
            {
                i += 1;
                output.WriteLine($"下载完成 {elapsed}");
            }).SetOnTransferFailed(e =>
            {
                i += 1;
                output.WriteLine($"下载出错 {e.Message}");
            });
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await fileDownloadManager.StartAsync();
        });
        Assert.Equal(2, i);

        File.Delete(destinationPath);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadInChunksAsync_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get,
                    new Uri(
                        "https://ts1.tc.mm.bing.net/th/id/R-C.987f582c510be58755c4933cda68d525?rik=C0D21hJDYvXosw&riu=http%3a%2f%2fimg.pconline.com.cn%2fimages%2fupload%2fupc%2ftx%2fwallpaper%2f1305%2f16%2fc4%2f20990657_1368686545122.jpg&ehk=netN2qzcCVS4ALUQfDOwxAwFcy41oxC%2b0xTFvOYy5ds%3d&risl=&pid=ImgRaw&r=0"))
                .SetDestinationPath(
                    @"C:\Workspaces\R-C.jpg").SetMaxThreads(2);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        await using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        await fileDownloadManager.DownloadInChunksAsync(840749, fileStream,
            new FileTransferProgress(@"C:\Workspaces\R-C.jpg", 21), new Stopwatch(), CancellationToken.None);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void DownloadInChunks_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get,
                    new Uri(
                        "https://ts1.tc.mm.bing.net/th/id/R-C.987f582c510be58755c4933cda68d525?rik=C0D21hJDYvXosw&riu=http%3a%2f%2fimg.pconline.com.cn%2fimages%2fupload%2fupc%2ftx%2fwallpaper%2f1305%2f16%2fc4%2f20990657_1368686545122.jpg&ehk=netN2qzcCVS4ALUQfDOwxAwFcy41oxC%2b0xTFvOYy5ds%3d&risl=&pid=ImgRaw&r=0"))
                .SetDestinationPath(
                    @"C:\Workspaces\R-C.jpg").SetMaxThreads(2);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        fileDownloadManager.DownloadInChunks(840749, fileStream,
            new FileTransferProgress(@"C:\Workspaces\R-C.jpg", 21), new Stopwatch(), CancellationToken.None);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task DownloadChunkAsync_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get,
                    new Uri(
                        "https://ts1.tc.mm.bing.net/th/id/R-C.987f582c510be58755c4933cda68d525?rik=C0D21hJDYvXosw&riu=http%3a%2f%2fimg.pconline.com.cn%2fimages%2fupload%2fupc%2ftx%2fwallpaper%2f1305%2f16%2fc4%2f20990657_1368686545122.jpg&ehk=netN2qzcCVS4ALUQfDOwxAwFcy41oxC%2b0xTFvOYy5ds%3d&risl=&pid=ImgRaw&r=0"))
                .SetDestinationPath(
                    @"C:\Workspaces\R-C.jpg").SetMaxThreads(2);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        await using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        var fileWriteLock = new SemaphoreSlim(1, 1);

        await fileDownloadManager.DownloadChunkAsync(0, 420374, fileStream, fileWriteLock,
            new FileTransferProgress(@"C:\Workspaces\R-C.jpg", 21), new Stopwatch(), CancellationToken.None);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void DownloadChunk_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get,
                    new Uri(
                        "https://ts1.tc.mm.bing.net/th/id/R-C.987f582c510be58755c4933cda68d525?rik=C0D21hJDYvXosw&riu=http%3a%2f%2fimg.pconline.com.cn%2fimages%2fupload%2fupc%2ftx%2fwallpaper%2f1305%2f16%2fc4%2f20990657_1368686545122.jpg&ehk=netN2qzcCVS4ALUQfDOwxAwFcy41oxC%2b0xTFvOYy5ds%3d&risl=&pid=ImgRaw&r=0"))
                .SetDestinationPath(
                    @"C:\Workspaces\R-C.jpg").SetMaxThreads(2);
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        var fileWriteLock = new SemaphoreSlim(1, 1);

        fileDownloadManager.DownloadChunk(0, 420374, fileStream, fileWriteLock,
            new FileTransferProgress(@"C:\Workspaces\R-C.jpg", 21), new Stopwatch(), CancellationToken.None);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task DownloadSingleThreadedAsync_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        await using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        await fileDownloadManager.DownloadSingleThreadedAsync(httpResponseMessage, fileStream,
            new FileTransferProgress(@"C:\Workspaces\index.html", 21), new Stopwatch(), CancellationToken.None);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void DownloadSingleThreaded_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            new HttpFileDownloadBuilder(HttpMethod.Get, new Uri("https://furion.net")).SetDestinationPath(
                @"C:\Workspaces\index.html");
        var fileDownloadManager = new FileDownloadManager(httpRemoteService, httpFileDownloadBuilder);

        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = new StringContent("测试文件内容", Encoding.UTF8, "text/plain");

        using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write,
            FileShare.Read,
            httpFileDownloadBuilder.BufferSize, true);

        fileDownloadManager.DownloadSingleThreaded(httpResponseMessage, fileStream,
            new FileTransferProgress(@"C:\Workspaces\index.html", 21), new Stopwatch(), CancellationToken.None);

        serviceProvider.Dispose();
    }
}