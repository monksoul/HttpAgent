// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var fileDownloadResult = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            _ = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath,
                cancellationToken: cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        // ReSharper disable once MethodHasAsyncOverload
        var fileDownloadResult = httpRemoteService.DownloadFile($"http://localhost:{port}/test", destinationPath,
            configure: downloadBuilder => downloadBuilder.WithRequest(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })));

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(1, i);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var fileDownloadResult =
            await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test", destinationPath);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test", destinationPath,
                cancellationToken: cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var fileDownloadResult = await httpRemoteService.DownloadFileAsync($"http://localhost:{port}/test",
            destinationPath,
            configure: downloadBuilder => downloadBuilder.WithRequest(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })));

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(1, i);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        // ReSharper disable once MethodHasAsyncOverload
        var fileDownloadResult = httpRemoteService.Send(httpFileDownloadBuilder);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            _ = httpRemoteService.Send(httpFileDownloadBuilder, cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
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

        await app.StartAsync();

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
        var fileDownloadResult = httpRemoteService.Send(httpFileDownloadBuilder);

        Assert.Equal(1, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var i = 0;
        // ReSharper disable once MethodHasAsyncOverload
        var fileDownloadResult = httpRemoteService.Send(httpFileDownloadBuilder.WithRequest(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })));

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(1, i);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        // ReSharper disable once MethodHasAsyncOverload
        var fileDownloadResult =
            httpRemoteService.Send(httpFileDownloadBuilder.SetEventHandler<CustomFileTransferEventHandler>());

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var fileDownloadResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder);

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsync(httpFileDownloadBuilder, cancellationTokenSource.Token);
        });

        Assert.False(File.Exists(destinationPath));

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath)
                .SetOnProgressChanged(async _ =>
                {
                    i += 1;
                    await Task.CompletedTask;
                });

        var fileDownloadResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder);

        Assert.Equal(1, i);
        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var i = 0;
        var fileDownloadResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder.WithRequest(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })));

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(1, i);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileDownloadBuilder =
            HttpRequestBuilder.DownloadFile(new Uri($"http://localhost:{port}/test"), destinationPath);

        var fileDownloadResult = await httpRemoteService.SendAsync(httpFileDownloadBuilder
            .SetEventHandler<CustomFileTransferEventHandler>());

        Assert.True(File.Exists(destinationPath));
        Assert.Equal(12, (await File.ReadAllBytesAsync(destinationPath)).Length);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        Assert.True(fileDownloadResult.IsSuccess);
        Assert.Equal(12, fileDownloadResult.FileSize);
        Assert.Equal(destinationPath, fileDownloadResult.FilePath);
        Assert.Equal(HttpStatusCode.OK, fileDownloadResult.StatusCode);
        Assert.False(fileDownloadResult.WasSkipped);
        Assert.Equal(FileExistsBehavior.CreateNew, fileDownloadResult.FileExistsBehavior);
        Assert.False(fileDownloadResult.UsedMultiThreadedDownload);
        Assert.True(fileDownloadResult.ElapsedMilliseconds > 0);
        Assert.Equal($"http://localhost:{port}/test", fileDownloadResult.RequestUri?.ToString());

        File.Delete(destinationPath);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath);
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            _ = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.UploadFile($"http://localhost:{port}/test", filePath,
            configure: uploadBuilder => uploadBuilder.WithRequest(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })));

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test", filePath);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test",
                filePath, cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpResponseMessage = await httpRemoteService.UploadFileAsync($"http://localhost:{port}/test",
            filePath, configure: uploadBuilder => uploadBuilder.WithRequest(requestBuilder =>
                requestBuilder.SetOnPreSendRequest(_ =>
                {
                    i += 1;
                })));

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder);
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            httpRemoteService.Send(httpFileUploadBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

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
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder);

        Assert.Equal(1, i);
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var i = 0;
        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpFileUploadBuilder.WithRequest(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })));

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal(1, i);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Send(httpFileUploadBuilder.SetEventHandler<CustomFileTransferEventHandler>());

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder);

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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
                await Task.Delay(200);

                await context.Response.WriteAsync(file.FileName);
            })
            .DisableAntiforgery(); // 禁用跨站攻击：https://learn.microsoft.com/en-us/dotnet/core/compatibility/aspnet-core/8.0/antiforgery-checks

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.SendAsync(httpFileUploadBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
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


        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var i = 0;
        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath).SetOnProgressChanged(async
                _ =>
            {
                i += 1;
                await Task.CompletedTask;
            });

        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder);

        Assert.Equal(1, i);
        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var i = 0;
        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder.WithRequest(requestBuilder =>
            requestBuilder.SetOnPreSendRequest(_ =>
            {
                i += 1;
            })));

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(1, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var customFileTransferEventHandler = new CustomFileTransferEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(fileTransferEventHandler: customFileTransferEventHandler);

        var httpFileUploadBuilder =
            HttpRequestBuilder.UploadFile(new Uri($"http://localhost:{port}/test"), filePath);

        var httpResponseMessage = await httpRemoteService.SendAsync(httpFileUploadBuilder
            .SetEventHandler<CustomFileTransferEventHandler>());

        var result = await httpResponseMessage!.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        Assert.Equal("test.txt", result);
        Assert.Equal(2, customFileTransferEventHandler.counter);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpServerSentEventsBuilder);

        Assert.Equal(5, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(120);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpServerSentEventsBuilder, cancellationTokenSource.Token);

        Assert.Equal(1, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

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

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpServerSentEventsBuilder);

        Assert.Equal(1, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

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

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpServerSentEventsBuilder);

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpServerSentEventsBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(6, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        await httpRemoteService.SendAsync(httpServerSentEventsBuilder);

        Assert.Equal(5, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(120);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpServerSentEventsBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

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

        await httpRemoteService.SendAsync(httpServerSentEventsBuilder);

        Assert.Equal(1, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

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

        await httpRemoteService.SendAsync(httpServerSentEventsBuilder);

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            });

        await httpRemoteService.SendAsync(httpServerSentEventsBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(6, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        });

        Assert.Equal(5, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(50);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Assert.True(e is OperationCanceledException);
        }

        Assert.Equal(0, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.ServerSentEvents($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        }, sseBuilder => sseBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(6, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        });

        Assert.Equal(5, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(120);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync();
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

            var eventId = 0;
            while (eventId < 5)
            {
                eventId++;

                var message = $"id: {eventId}\nevent: update\ndata: Message {eventId} at {DateTime.UtcNow}\n\n";
                await context.Response.WriteAsync(message);

                await Task.Delay(10);
            }
        });

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.ServerSentEventsAsync($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        }, sseBuilder => sseBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(6, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Send(httpStressTestHarnessBuilder);
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpStressTestHarnessBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Send(httpStressTestHarnessBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        Assert.Equal(10, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        var result = await httpRemoteService.SendAsync(httpStressTestHarnessBuilder);
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpStressTestHarnessBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);

        var i = 0;
        var result = await httpRemoteService.SendAsync(httpStressTestHarnessBuilder.WithRequest(b =>
            b.SetOnPreSendRequest(_ =>
            {
                i++;
            })));
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        Assert.Equal(10, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10);
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.StressTestHarness($"http://localhost:{port}/test", 10, sthBuilder =>
            sthBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })));
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 15);

        Assert.Equal(10, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10);
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10,
                cancellationToken: cancellationTokenSource.Token);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.StressTestHarnessAsync($"http://localhost:{port}/test", 10,
            sthBuilder => sthBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
            {
                i++;
            })));
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        Assert.Equal(10, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpLongPollingBuilder);

        Assert.Equal(5, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.Send(httpLongPollingBuilder, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Assert.True(e is OperationCanceledException);
        }

        Assert.Equal(0, i);

        await app.StopAsync();
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

        await app.StartAsync();

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
        httpRemoteService.Send(httpLongPollingBuilder);

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.Send(httpLongPollingBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(11, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        await httpRemoteService.SendAsync(httpLongPollingBuilder);

        Assert.Equal(5, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.SendAsync(httpLongPollingBuilder, cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync();
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

        await app.StartAsync();

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

        await httpRemoteService.SendAsync(httpLongPollingBuilder);

        Assert.Equal(5, i);
        Assert.Equal(5, customLongPollingEventHandler.counter);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetOnDataReceived(async (_, _) =>
                {
                    i++;
                    await Task.CompletedTask;
                });

        await httpRemoteService.SendAsync(httpLongPollingBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(11, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.LongPolling($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        });

        Assert.Equal(5, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        try
        {
            // ReSharper disable once MethodHasAsyncOverload
            httpRemoteService.LongPolling($"http://localhost:{port}/test", async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Assert.True(e is OperationCanceledException);
        }

        Assert.Equal(0, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        httpRemoteService.LongPolling($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        }, lpBuilder => lpBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(11, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        });

        Assert.Equal(5, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", async (_, _) =>
            {
                i++;
                await Task.CompletedTask;
            }, cancellationToken: cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await httpRemoteService.LongPollingAsync($"http://localhost:{port}/test", async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        }, lpBuilder => lpBuilder.WithRequest(b => b.SetOnPreSendRequest(_ =>
        {
            i++;
        })));

        Assert.Equal(11, i);

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.Declarative(method, [$"http://localhost:{port}/test", CancellationToken.None]);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Declarative(method, [$"http://localhost:{port}/test", cancellationTokenSource.Token]);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        var result =
            await httpRemoteService.DeclarativeAsync<string>(method,
                [$"http://localhost:{port}/test", CancellationToken.None]);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.DeclarativeAsync<string>(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token]);
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.SendAs(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", CancellationToken.None]));
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token]));
        });

        await app.StopAsync();
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

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        var result =
            await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", CancellationToken.None]));
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync<string>(HttpRequestBuilder.Declarative(method,
                [$"http://localhost:{port}/test", cancellationTokenSource.Token]));
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}