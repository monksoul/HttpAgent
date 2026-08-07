// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

[Collection("HttpRemoteServiceExtensionsTests")]
public class HttpRemoteServiceExtensionsTests
{
    [Fact]
    public async Task DownloadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFile_WithCancellationToken_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test3.txt");
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
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            _ = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath,
                cancellationToken: cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath,
            configure: downloadBuilder => downloadBuilder.With(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(1, i);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFileWithConsoleProgress_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult =
            httpRemoteService.DownloadFileWithConsoleProgress($"http://localhost:{port}/test", destinationPath,
                cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFileAsync_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var fileTransferResult =
            await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test", destinationPath,
                cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFileAsync_WithCancellationToken_ReturnOK()
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
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ = await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test", destinationPath,
                cancellationToken: cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFileAsync_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var fileTransferResult = await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test",
            destinationPath, configure: downloadBuilder => downloadBuilder.With(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(1, i);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DownloadFileWithConsoleProgressAsync_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var fileTransferResult =
            await httpRemoteService.DownloadFileWithConsoleProgressAsync($"http://localhost:{port}/test",
                destinationPath, cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_DownloadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult = httpRemoteService.Send(httpFileDownloadBuilder, TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_DownloadFile_WithCancellationToken_ReturnOK()
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
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            _ = httpRemoteService.Send(httpFileDownloadBuilder, cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_DownloadFile_WithOnProgressChanged_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                });

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult = httpRemoteService.Send(httpFileDownloadBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(2, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_DownloadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);
        var i = 0;

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult = httpRemoteService.Send(httpFileDownloadBuilder.With(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })), TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(1, i);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_DownloadFile_EventHandler_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        // ReSharper disable once MethodHasAsyncOverload
        var fileTransferResult =
            httpRemoteService.Send(httpFileDownloadBuilder.SetEventHandler<CustomFileTransferEventHandler>(),
                TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_DownloadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var fileTransferResult =
            await httpRemoteService.SendAsync(httpFileDownloadBuilder, TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_DownloadFile_WithCancellationToken_ReturnOK()
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
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsync(httpFileDownloadBuilder, cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_DownloadFile_WithOnProgressChanged_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                });

        var fileTransferResult =
            await httpRemoteService.SendAsync(httpFileDownloadBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(2, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_DownloadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);
        var i = 0;

        var fileTransferResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder.With(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })), TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(1, i);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_DownloadFile_EventHandler_ReturnOK()
    {
        var destinationPath = Path.Combine(AppContext.BaseDirectory, "downloads", "test4.txt");
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var fileTransferResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder
            .SetEventHandler<CustomFileTransferEventHandler>(), TestContext.Current.CancellationToken);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath, TestContext.Current.CancellationToken)).Length);
        Assert.Equal(2, customFileTransferEventHandler.counter);
        Assert.True(fileTransferResult.IsSuccess);
        Assert.Equal(12, fileTransferResult.FileSize);
        Assert.Equal(destinationPath, fileTransferResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileTransferResult.StatusCode);
        Assert.True(fileTransferResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileTransferResult.RequestUri?.ToString());

        File.Delete(destinationPath);
        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath,
            cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFile_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200, TestContext.Current.CancellationToken);
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            _ = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath,
            configure: uploadBuilder => uploadBuilder.With(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })), cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFileWithConsoleProgress_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.UploadFileWithConsoleProgress($"http://localhost:{port}/test", filePath,
                cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFileAsync_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test", filePath,
                cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFileAsync_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200, TestContext.Current.CancellationToken);
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ = await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test",
                filePath, cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFileAsync_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var httpResponseMessage = await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test", filePath,
            configure: uploadBuilder => uploadBuilder.With(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })), cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task UploadFileWithConsoleProgressAsync_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.UploadFileWithConsoleProgressAsync($"http://localhost:{port}/test", filePath,
                cancellationToken: TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_UploadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder, TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_UploadFile_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200, TestContext.Current.CancellationToken);
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            httpRemoteService.Send(httpFileUploadBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_UploadFile_WithOnProgressChanged_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(1, i);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_UploadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);
        var i = 0;

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder.With(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })), TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal(1, i);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_UploadFile_EventHandler_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Send(httpFileUploadBuilder.SetEventHandler<CustomFileTransferEventHandler>(),
                TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_UploadFile_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var httpResponseMessage =
            await httpRemoteService.SendAsync(httpFileUploadBuilder, TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_UploadFile_WithCancellationToken_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "test.txt");
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async (HttpContext context, IFormFile file) =>
            {
                await Task.Delay(200, TestContext.Current.CancellationToken);
                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.SendAsync(httpFileUploadBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_UploadFile_WithOnProgressChanged_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var i = 0;

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });

        var httpResponseMessage =
            await httpRemoteService.SendAsync(httpFileUploadBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(1, i);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_UploadFile_WithHttpRequestBuilder_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);
        var i = 0;

        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder.With(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })), TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_UploadFile_EventHandler_ReturnOK()
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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder
            .SetEventHandler<CustomFileTransferEventHandler>(), TestContext.Current.CancellationToken);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_ServerSentEvents_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            });

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_ServerSentEvents_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(120, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                return Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpServerSentEventsBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_ServerSentEvents_Filter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnOpen(() =>
            {
                i++;
            }).SetOnError(_ =>
            {
                i++;
            });

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_ServerSentEvents_EventHandler_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        var i = 0;
        var customServerSentEventsEventHandler = new CustomServerSentEventsEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(sentEventsEventHandler: customServerSentEventsEventHandler);

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnOpen(() =>
                {
                    i++;
                }).SetOnError(_ =>
                {
                    i++;
                })
                .SetEventHandler<CustomServerSentEventsEventHandler>();

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_ServerSentEvents_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            });

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpServerSentEventsBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cts.Token);
        });

        Assert.Equal(6, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ServerSentEvents_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ServerSentEvents_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(120, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                return Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ServerSentEvents_Filter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnOpen(() =>
            {
                i++;
            }).SetOnError(_ =>
            {
                i++;
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ServerSentEvents_EventHandler_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        var i = 0;
        var customServerSentEventsEventHandler = new CustomServerSentEventsEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(sentEventsEventHandler: customServerSentEventsEventHandler);

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnOpen(() =>
                {
                    i++;
                }).SetOnError(_ =>
                {
                    i++;
                })
                .SetEventHandler<CustomServerSentEventsEventHandler>();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder, cts.Token);
        });

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ServerSentEvents_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cts.Token);
        });

        Assert.Equal(6, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEvents_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            }, cancellationToken: cts.Token);
        });

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEvents_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(100);
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(50, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEvents_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            }, sseBuilder => sseBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cts.Token);
        });

        Assert.Equal(6, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEventsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            }, cancellationToken: cts.Token);
        });

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEventsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(120, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEventsAsync_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }

                return Task.CompletedTask;
            }, sseBuilder => sseBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cts.Token);
        });

        Assert.Equal(6, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_StressTestHarness_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Send(httpStressTestHarnessBuilder,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_StressTestHarness_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpStressTestHarnessBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_StressTestHarness_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Send(httpStressTestHarnessBuilder.With(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);
        Assert.Equal(10, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_StressTestHarness_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        var result = await httpRemoteService.SendAsync(httpStressTestHarnessBuilder,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_StressTestHarness_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpStressTestHarnessBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_StressTestHarness_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);
        var i = 0;

        var result = await httpRemoteService.SendAsync(httpStressTestHarnessBuilder.With(b =>
            b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);
        Assert.Equal(10, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarness_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarness_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarness_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10, sthBuilder =>
            sthBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 15);
        Assert.Equal(10, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarnessAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarnessAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StressTestHarnessAsync_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello Furion";
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10, sthBuilder =>
            sthBuilder.With(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })), cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);
        Assert.Equal(10, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_LongPolling_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpLongPollingBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_LongPolling_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpLongPollingBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_LongPolling_EventHandler_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var customLongPollingEventHandler = new CustomLongPollingEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(longPollingEventHandler: customLongPollingEventHandler);

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                })
                .SetEventHandler<CustomLongPollingEventHandler>();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpLongPollingBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_LongPolling_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpLongPollingBuilder.With(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })), TestContext.Current.CancellationToken);

        // 5次 DataReceived + 6次 PreSendRequest (包含最后一次获取 X-End-Of-Stream 的请求) = 11
        Assert.Equal(11, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_LongPolling_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        await httpRemoteService.SendAsync(httpLongPollingBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_LongPolling_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpLongPollingBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_LongPolling_EventHandler_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var customLongPollingEventHandler = new CustomLongPollingEventHandler();

        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(longPollingEventHandler: customLongPollingEventHandler);

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                })
                .SetEventHandler<CustomLongPollingEventHandler>();

        await httpRemoteService.SendAsync(httpLongPollingBuilder, TestContext.Current.CancellationToken);

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_LongPolling_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });

        await httpRemoteService.SendAsync(httpLongPollingBuilder.With(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })), TestContext.Current.CancellationToken);

        Assert.Equal(11, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPolling_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.LongPolling($"http://localhost:{port}/test", (_, _) =>
        {
            i++;
            return Task.CompletedTask;
        }, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPolling_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.LongPolling($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPolling_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.LongPolling($"http://localhost:{port}/test", (_, _) =>
        {
            i++;
            return Task.CompletedTask;
        }, lpBuilder => lpBuilder.With(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })), TestContext.Current.CancellationToken);

        Assert.Equal(11, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPollingAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", (_, _) =>
        {
            i++;
            return Task.CompletedTask;
        }, cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPollingAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(120, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", (_, _) =>
            {
                i++;
                return Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPollingAsync_WithHttpRequestBuilder_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", (_, _) =>
        {
            i++;
            return Task.CompletedTask;
        }, lpBuilder => lpBuilder.With(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })), TestContext.Current.CancellationToken);

        Assert.Equal(11, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Declarative_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Declarative(method, [$"http://localhost:{port}/test", CancellationToken.None],
            typeof(IHttpDeclarativeTest));

        Assert.Equal("Hello World!", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Declarative_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Declarative(method, [$"http://localhost:{port}/test", cancellationTokenSource.Token],
                typeof(IHttpDeclarativeTest));
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeclarativeAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        var result = await httpRemoteService.DeclarativeAsync<string>(method,
            [$"http://localhost:{port}/test", CancellationToken.None], typeof(IHttpDeclarativeTest));

        Assert.Equal("Hello World!", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeclarativeAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ = await httpRemoteService.DeclarativeAsync<string>(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token], typeof(IHttpDeclarativeTest));
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithDeclarative_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.SendAs(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", CancellationToken.None], typeof(IHttpDeclarativeTest)));

        Assert.Equal("Hello World!", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithDeclarative_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token], typeof(IHttpDeclarativeTest)));
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithDeclarativeAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        var result =
            await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", CancellationToken.None], typeof(IHttpDeclarativeTest)));

        Assert.Equal("Hello World!", result);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithDeclarativeAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(200, TestContext.Current.CancellationToken);
            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token], typeof(IHttpDeclarativeTest)));
        });

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsyncEnumerable_LongPolling_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        try
        {
            await foreach (var _ in httpRemoteService.SendAsAsyncEnumerable(httpLongPollingBuilder, cts.Token))
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsyncEnumerable_ServerSentEvents_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"));

        try
        {
            await foreach (var _ in httpRemoteService.SendAsAsyncEnumerable(httpServerSentEventsBuilder, cts.Token))
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ServerSentEventsAsAsyncEnumerable_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            try
            {
                var eventId = 0;
                while (eventId < 5 && !context.RequestAborted.IsCancellationRequested)
                {
                    eventId++;
                    var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                    await context.Response.WriteAsync(message, context.RequestAborted);
                    await Task.Delay(10, context.RequestAborted);
                }

                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
            catch (OperationCanceledException) { }
            catch (IOException) { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        try
        {
            await foreach (var _ in httpRemoteService.ServerSentEventsAsAsyncEnumerable($"http://localhost:{port}/test",
                               cancellationToken: cts.Token))
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task LongPollingAsAsyncEnumerable_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var j = 0;
        app.MapGet("/test", async context =>
        {
            j++;
            var message = $"Message at {DateTime.UtcNow}\n\n";
            await Task.Delay(50, context.RequestAborted);

            if (j <= 5)
            {
                await context.Response.WriteAsync(message);
            }
            else
            {
                context.Response.Headers["X-End-Of-Stream"] = "1";
                if (!context.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(Timeout.Infinite, context.RequestAborted);
                }
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        using var cts = new CancellationTokenSource();
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        try
        {
            await foreach (var _ in httpRemoteService.LongPollingAsAsyncEnumerable($"http://localhost:{port}/test",
                               cancellationToken: cts.Token))
            {
                i++;
                if (i >= 5)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException) { }

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }
}