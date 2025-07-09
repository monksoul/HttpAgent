// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ServerSentEventsManagerTests(ITestOutputHelper output)
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new ServerSentEventsManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new ServerSentEventsManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService,
            new HttpServerSentEventsBuilder(new Uri("http://localhost:5000")));

        Assert.NotNull(serverSentEventsManager._httpServerSentEventsBuilder);
        Assert.NotNull(serverSentEventsManager._httpRemoteService);
        Assert.NotNull(serverSentEventsManager.RequestBuilder);
        Assert.Null(serverSentEventsManager.ServerSentEventsEventHandler);
        Assert.Equal(2000, serverSentEventsManager.CurrentRetryInterval);
        Assert.Equal(0, serverSentEventsManager.CurrentRetries);

        var serverSentEventsManager2 = new ServerSentEventsManager(httpRemoteService,
            new HttpServerSentEventsBuilder(new Uri("http://localhost:5000")).WithRequest(builder =>
                builder.SetTimeout(100)));
        Assert.NotNull(serverSentEventsManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), serverSentEventsManager2.RequestBuilder.Timeout);

        serviceProvider.Dispose();
    }

    [Fact]
    public void IsEventComplete_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ServerSentEventsManager.IsEventComplete(null!));

    [Fact]
    public void IsEventComplete_ReturnOK()
    {
        Assert.False(ServerSentEventsManager.IsEventComplete(new ServerSentEventsData()));

        var serverSentEventsData = new ServerSentEventsData();
        serverSentEventsData.AppendData("data");
        Assert.True(ServerSentEventsManager.IsEventComplete(serverSentEventsData));
    }

    [Fact]
    public void TryParseEventLine_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService,
            new HttpServerSentEventsBuilder(new Uri("http://localhost:5000")));

        ServerSentEventsData? serverSentEventsData = null;
        var result = serverSentEventsManager.TryParseEventLine(null!, ref serverSentEventsData);
        Assert.False(result);
        Assert.Null(serverSentEventsData);

        ServerSentEventsData? serverSentEventsData2 = null;
        var result2 = serverSentEventsManager.TryParseEventLine(string.Empty, ref serverSentEventsData2);
        Assert.False(result2);
        Assert.Null(serverSentEventsData2);

        ServerSentEventsData? serverSentEventsData3 = null;
        var result3 = serverSentEventsManager.TryParseEventLine(" ", ref serverSentEventsData3);
        Assert.False(result3);
        Assert.Null(serverSentEventsData3);

        ServerSentEventsData? serverSentEventsData4 = null;
        var result4 = serverSentEventsManager.TryParseEventLine(":这是一行注释", ref serverSentEventsData4);
        Assert.False(result4);
        Assert.Null(serverSentEventsData4);

        ServerSentEventsData? serverSentEventsData5 = null;
        var result5 = serverSentEventsManager.TryParseEventLine("data: 这是一行数据", ref serverSentEventsData5);
        Assert.True(result5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("这是一行数据", serverSentEventsData5.Data);

        var result6 = serverSentEventsManager.TryParseEventLine("event: myname", ref serverSentEventsData5);
        Assert.True(result6);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("myname", serverSentEventsData5.Event);

        var result7 = serverSentEventsManager.TryParseEventLine("id: myid", ref serverSentEventsData5);
        Assert.True(result7);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("myid", serverSentEventsData5.Id);

        var result8 = serverSentEventsManager.TryParseEventLine("retry: 1000", ref serverSentEventsData5);
        Assert.True(result8);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal(1000, serverSentEventsData5.Retry);

        var result9 = serverSentEventsManager.TryParseEventLine("retry: some", ref serverSentEventsData5);
        Assert.True(result9);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal(2000, serverSentEventsData5.Retry);

        var result10 = serverSentEventsManager.TryParseEventLine("some: ok", ref serverSentEventsData5);
        Assert.True(result10);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("这是一行数据", serverSentEventsData5.Data);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task ReceiveDataAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await serverSentEventsManager.ReceiveDataAsync(null!, CancellationToken.None));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ReceiveDataAsync_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnMessage(async (_, _) =>
            {
                i += 1;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var messageChannel = Channel.CreateUnbounded<ServerSentEventsData>();
        using var messageCancellationTokenSource = new CancellationTokenSource();
        var receiveDataTask =
            serverSentEventsManager.ReceiveDataAsync(messageChannel, messageCancellationTokenSource.Token);

        for (var j = 0; j < 3; j++)
        {
            await messageChannel.Writer.WriteAsync(
                new ServerSentEventsData(), messageCancellationTokenSource.Token);
        }

        await Task.Delay(200, messageCancellationTokenSource.Token);

        messageChannel.Writer.Complete();

        await messageCancellationTokenSource.CancelAsync();
        await receiveDataTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task ReceiveDataAsync_WithSetOnProgressChanged_Exception_ReturnOK()
    {
        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnMessage(async (_, _) =>
            {
                i += 1;
                throw new Exception("Error");
#pragma warning disable CS0162 // 检测到不可到达的代码
                await Task.CompletedTask;
#pragma warning restore CS0162 // 检测到不可到达的代码
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var messageChannel = Channel.CreateUnbounded<ServerSentEventsData>();
        using var messageCancellationTokenSource = new CancellationTokenSource();
        var receiveDataTask =
            serverSentEventsManager.ReceiveDataAsync(messageChannel, messageCancellationTokenSource.Token);

        for (var j = 0; j < 3; j++)
        {
            await messageChannel.Writer.WriteAsync(
                new ServerSentEventsData(), messageCancellationTokenSource.Token);
        }

        await Task.Delay(200, messageCancellationTokenSource.Token);

        messageChannel.Writer.Complete();

        await messageCancellationTokenSource.CancelAsync();
        await receiveDataTask;

        Assert.Equal(3, i);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void HandleOpen_ReturnOK()
    {
        var customServerSentEventsEventHandler = new CustomServerSentEventsEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(sentEventsEventHandler: customServerSentEventsEventHandler);
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnOpen(() => throw new Exception("出错了"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        serverSentEventsManager.HandleOpen();

        serviceProvider.Dispose();
    }

    [Fact]
    public void HandleError_ReturnOK()
    {
        var customServerSentEventsEventHandler = new CustomServerSentEventsEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(sentEventsEventHandler: customServerSentEventsEventHandler);
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnError(_ => throw new Exception("出错了"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        serverSentEventsManager.HandleError(new Exception("出错了"));

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task HandleMessageReceivedAsync_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await serverSentEventsManager.HandleMessageReceivedAsync(null!));

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HandleMessageReceivedAsync_ReturnOK()
    {
        var customServerSentEventsEventHandler = new CustomServerSentEventsEventHandler();
        var (httpRemoteService, serviceProvider) =
            Helpers.CreateHttpRemoteService(sentEventsEventHandler: customServerSentEventsEventHandler);
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri("https://furion.net"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var serverSentEventsData =
            new ServerSentEventsData();
        await serverSentEventsManager.HandleMessageReceivedAsync(serverSentEventsData);

        var i = 0;
        httpServerSentEventsBuilder.SetOnMessage(async (_, _) =>
        {
            i++;
            await Task.CompletedTask;
        });

        var serverSentEventsManager2 = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);
        await serverSentEventsManager2.HandleMessageReceivedAsync(serverSentEventsData);

        Assert.Equal(1, i);
        Assert.Equal(0, customServerSentEventsEventHandler.counter);

        httpServerSentEventsBuilder.SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager3 = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);
        await serverSentEventsManager3.HandleMessageReceivedAsync(serverSentEventsData);

        Assert.Equal(1, customServerSentEventsEventHandler.counter);

        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_ReturnOK()
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        serverSentEventsManager.Start();

        Assert.Equal(5, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_WithCancellationToken_ReturnOK()
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        // ReSharper disable once MethodHasAsyncOverload
        serverSentEventsManager.Start(cancellationTokenSource.Token);

        Assert.Equal(1, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_Filter_ReturnOK()
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
                output.WriteLine("准备连接...");
            }).SetOnError(e =>
            {
                i++;
                output.WriteLine("连接失败...");
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        serverSentEventsManager.Start();

        Assert.Equal(1, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_EventHandler_ReturnOK()
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
                    output.WriteLine("准备连接...");
                }).SetOnError(e =>
                {
                    i++;
                    output.WriteLine("连接失败...");
                })
                .SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        serverSentEventsManager.Start();

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_ReturnOK()
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await serverSentEventsManager.StartAsync();

        Assert.Equal(5, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithCancellationToken_ReturnOK()
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
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await serverSentEventsManager.StartAsync(cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_Filter_ReturnOK()
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
                output.WriteLine("准备连接...");
            }).SetOnError(e =>
            {
                i++;
                output.WriteLine("连接失败...");
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await serverSentEventsManager.StartAsync();

        Assert.Equal(1, i);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_EventHandler_ReturnOK()
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
                    output.WriteLine("准备连接...");
                }).SetOnError(e =>
                {
                    i++;
                    output.WriteLine("连接失败...");
                })
                .SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await serverSentEventsManager.StartAsync();

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Retry_ReturnOK()
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var stopwatch = Stopwatch.StartNew();
        // ReSharper disable once MethodHasAsyncOverload
        serverSentEventsManager.Retry();
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds > 2000);

        Assert.Equal(5, i);
        Assert.Equal(0, serverSentEventsManager.CurrentRetries);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task RetryAsync_ReturnOK()
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage(async (data, _) =>
            {
                i++;
                await Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var stopwatch = Stopwatch.StartNew();
        await serverSentEventsManager.RetryAsync();
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds > 2000);

        Assert.Equal(5, i);
        Assert.Equal(0, serverSentEventsManager.CurrentRetries);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}