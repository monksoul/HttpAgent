// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class FileUploadManagerTests(ITestOutputHelper output)
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new FileUploadManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new FileUploadManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var fileUploadManager = new FileUploadManager(httpRemoteService,
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost:5000"), filePath, "file"));

        Assert.NotNull(fileUploadManager._httpFileUploadBuilder);
        Assert.NotNull(fileUploadManager._httpRemoteService);
        Assert.NotNull(fileUploadManager._progressChannel);
        Assert.Equal("UnboundedChannel`1", fileUploadManager._progressChannel.GetType().Name);
        Assert.NotNull(fileUploadManager.RequestBuilder);
        Assert.Null(fileUploadManager.FileTransferEventHandler);

        var fileUploadManager2 = new FileUploadManager(httpRemoteService,
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("http://localhost:5000"), filePath, "file")
                .WithRequest(builder => builder.SetTimeout(100)));
        Assert.NotNull(fileUploadManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), fileUploadManager2.RequestBuilder.Timeout);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task ReportProgressAsync_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file")
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                }).SetProgressInterval(TimeSpan.FromMilliseconds(50));
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        using var progressCancellationTokenSource = new CancellationTokenSource();
        var reportProgressTask =
            fileUploadManager.ReportProgressAsync(progressCancellationTokenSource.Token, CancellationToken.None);

        for (var j = 0; j < 3; j++)
        {
            await fileUploadManager._progressChannel.Writer.WriteAsync(
                new FileTransferProgress(filePath, -1), progressCancellationTokenSource.Token);
        }

        await Task.Delay(200, progressCancellationTokenSource.Token);

        fileUploadManager._progressChannel.Writer.Complete();

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

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file")
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    throw new Exception("Error");
#pragma warning disable CS0162 // 检测到不可到达的代码
                    await Task.CompletedTask;
#pragma warning restore CS0162 // 检测到不可到达的代码
                }).SetProgressInterval(TimeSpan.FromMilliseconds(50));
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        using var progressCancellationTokenSource = new CancellationTokenSource();
        var reportProgressTask =
            fileUploadManager.ReportProgressAsync(progressCancellationTokenSource.Token, CancellationToken.None);

        for (var j = 0; j < 3; j++)
        {
            await fileUploadManager._progressChannel.Writer.WriteAsync(
                new FileTransferProgress(filePath, -1), progressCancellationTokenSource.Token);
        }

        await Task.Delay(200, progressCancellationTokenSource.Token);

        fileUploadManager._progressChannel.Writer.Complete();

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

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file")
                .SetOnTransferStarted(() => throw new Exception("出错了"));
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        fileUploadManager.HandleTransferStarted();

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleTransferCompleted_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file")
                .SetOnTransferCompleted(_ => throw new Exception("出错了"));
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        fileUploadManager.HandleTransferCompleted(100);

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleTransferFailed_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file")
                .SetOnTransferFailed(_ => throw new Exception("出错了"));
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        fileUploadManager.HandleTransferFailed(new Exception("出错了"));

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task HandleProgressChangedAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fileUploadManager.HandleProgressChangedAsync(null!));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HandleProgressChangedAsync_ReturnOK()
    {
        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri("https://furion.net"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        var fileTransferProgress =
            new FileTransferProgress(filePath, -1);
        await fileUploadManager.HandleProgressChangedAsync(fileTransferProgress);

        var i = 0;
        httpFileUploadBuilder.SetOnProgressChanged(async _ =>
        {
            i++;
            await Task.CompletedTask;
        });

        var fileUploadManager2 = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);
        await fileUploadManager2.HandleProgressChangedAsync(fileTransferProgress);

        Assert.Equal(1, i);
        Assert.Equal(0, customFileTransferEventHandler.counter2);

        httpFileUploadBuilder.SetEventHandler<CustomFileTransferEventHandler>();
        var fileUploadManager3 = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);
        await fileUploadManager3.HandleProgressChangedAsync(fileTransferProgress);

        Assert.Equal(1, customFileTransferEventHandler.counter2);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = fileUploadManager.Start();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            _ = fileUploadManager.Start(cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithOnProgressChanged_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = fileUploadManager.Start();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(1, i);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_Filter_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = fileUploadManager.Start();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(2, i);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_EventHandler_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                }).SetEventHandler<CustomFileTransferEventHandler>();
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = fileUploadManager.Start();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(2, i);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithTransferFailed_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);
                throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
                await context.Response.WriteAsync(file.FileName);
#pragma warning restore CS0162 // 检测到不可到达的代码
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                }).SetOnTransferFailed(e =>
                {
                    i += 1;
                    output.WriteLine($"上传出错 {e.Message}");
                }).WithRequest(b =>
                {
                    b.EnsureSuccessStatusCode();
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        Assert.Throws<HttpRequestException>(() =>
        {
            _ = fileUploadManager.Start();
        });
        Assert.Equal(2, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        var httpResponseMessage = await fileUploadManager.StartAsync();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file");
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await fileUploadManager.StartAsync(cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithOnProgressChanged_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        var httpResponseMessage = await fileUploadManager.StartAsync();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(1, i);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_Filter_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        var httpResponseMessage = await fileUploadManager.StartAsync();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(2, i);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_EventHandler_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                }).SetEventHandler<CustomFileTransferEventHandler>();
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        var httpResponseMessage = await fileUploadManager.StartAsync();
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        output.WriteLine(result);

        Assert.Equal(2, i);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithTransferFailed_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");

        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(50);
                throw new Exception("出错了");

#pragma warning disable CS0162 // 检测到不可到达的代码
                await context.Response.WriteAsync(file.FileName);
#pragma warning restore CS0162 // 检测到不可到达的代码
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            new HttpFileUploadBuilder(HttpMethod.Post, new Uri($"http://localhost:{port}/test"), filePath, "file")
                .SetOnTransferStarted(() =>
                {
                    i += 1;
                    output.WriteLine("准备上传...");
                }).SetOnTransferCompleted(elapsed =>
                {
                    i += 1;
                    output.WriteLine($"上传完成 {elapsed}");
                }).SetOnTransferFailed(e =>
                {
                    i += 1;
                    output.WriteLine($"上传出错 {e.Message}");
                }).WithRequest(b =>
                {
                    b.EnsureSuccessStatusCode();
                });
        var fileUploadManager = new FileUploadManager(httpRemoteService, httpFileUploadBuilder);

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            _ = await fileUploadManager.StartAsync();
        });
        Assert.Equal(2, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}