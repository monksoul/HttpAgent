// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ServerSentEventsManagerTests
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
            new HttpServerSentEventsBuilder(new Uri("http://localhost:5000")).With(builder =>
                builder.SetTimeout(100)));
        Assert.NotNull(serverSentEventsManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), serverSentEventsManager2.RequestBuilder.TimeoutOptions?.Timeout);

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
        serverSentEventsManager.TryParseEventLine(null!, ref serverSentEventsData);
        Assert.Null(serverSentEventsData);

        ServerSentEventsData? serverSentEventsData2 = null;
        serverSentEventsManager.TryParseEventLine(string.Empty, ref serverSentEventsData2);
        Assert.Null(serverSentEventsData2);

        ServerSentEventsData? serverSentEventsData3 = null;
        serverSentEventsManager.TryParseEventLine(" ", ref serverSentEventsData3);
        Assert.Null(serverSentEventsData3);

        ServerSentEventsData? serverSentEventsData4 = null;
        serverSentEventsManager.TryParseEventLine(":这是一行注释", ref serverSentEventsData4);
        Assert.Null(serverSentEventsData4);

        ServerSentEventsData? serverSentEventsData5 = null;
        serverSentEventsManager.TryParseEventLine("data: 这是一行数据", ref serverSentEventsData5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("这是一行数据", serverSentEventsData5.Data);
        Assert.Equal("data: 这是一行数据", serverSentEventsData5.RawLine);

        serverSentEventsManager.TryParseEventLine("event: myname", ref serverSentEventsData5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("myname", serverSentEventsData5.Event);

        serverSentEventsManager.TryParseEventLine("id: myid", ref serverSentEventsData5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal("myid", serverSentEventsData5.Id);

        serverSentEventsManager.TryParseEventLine("retry: 1000", ref serverSentEventsData5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal(1000, serverSentEventsData5.Retry);

        serverSentEventsManager.TryParseEventLine("retry: some", ref serverSentEventsData5);
        Assert.NotNull(serverSentEventsData5);
        Assert.Equal(2000, serverSentEventsData5.Retry);

        serverSentEventsManager.TryParseEventLine("some: ok", ref serverSentEventsData5);
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
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnMessage((_, _) =>
            {
                i += 1;
                return Task.CompletedTask;
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
            new HttpServerSentEventsBuilder(new Uri("https://furion.net")).SetOnMessage((_, _) =>
            {
                i += 1;
                throw new Exception("Error");
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
            await serverSentEventsManager.HandleMessageReceivedAsync(null!, TestContext.Current.CancellationToken));

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
        await serverSentEventsManager.HandleMessageReceivedAsync(serverSentEventsData,
            TestContext.Current.CancellationToken);

        var i = 0;
        httpServerSentEventsBuilder.SetOnMessage((_, _) =>
        {
            i++;
            return Task.CompletedTask;
        });

        var serverSentEventsManager2 = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);
        await serverSentEventsManager2.HandleMessageReceivedAsync(serverSentEventsData,
            TestContext.Current.CancellationToken);

        Assert.Equal(1, i);
        Assert.Equal(0, customServerSentEventsEventHandler.counter);

        httpServerSentEventsBuilder.SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager3 = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);
        await serverSentEventsManager3.HandleMessageReceivedAsync(serverSentEventsData,
            TestContext.Current.CancellationToken);

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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnMessage((_, _) =>
                {
                    i++;
                    if (i >= 5)
                    {
                        cts.Cancel();
                    }

                    return Task.CompletedTask;
                });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        Assert.ThrowsAny<OperationCanceledException>(() => serverSentEventsManager.Start(cts.Token));

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.ThrowsAny<OperationCanceledException>(() =>
        {
            serverSentEventsManager.Start(cancellationTokenSource.Token);
        });

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnOpen(() => i++)
                .SetOnError(_ => i++);
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        Assert.ThrowsAny<OperationCanceledException>(() => serverSentEventsManager.Start(cts.Token));

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
                .SetOnOpen(() => i++)
                .SetOnError(_ => i++)
                .SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        Assert.ThrowsAny<OperationCanceledException>(() => serverSentEventsManager.Start(cts.Token));

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnMessage((_, _) =>
                {
                    i++;
                    if (i >= 5)
                    {
                        cts.Cancel();
                    }

                    return Task.CompletedTask;
                });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await serverSentEventsManager.StartAsync(cts.Token));

        Assert.Equal(5, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test")).SetOnMessage((_, _) =>
            {
                i++;
                return Task.CompletedTask;
            });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(10);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await serverSentEventsManager.StartAsync(cancellationTokenSource.Token);
        });

        Assert.Equal(0, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnOpen(() => i++)
                .SetOnError(_ => i++);
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await serverSentEventsManager.StartAsync(cts.Token));

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
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
                .SetOnOpen(() => i++)
                .SetOnError(_ => i++)
                .SetEventHandler<CustomServerSentEventsEventHandler>();
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await serverSentEventsManager.StartAsync(cts.Token));

        Assert.Equal(1, i);
        Assert.Equal(6, customServerSentEventsEventHandler.counter);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsAsyncEnumerable_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
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
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        try
        {
            await foreach (var _ in serverSentEventsManager.StartAsAsyncEnumerable(cts.Token))
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
    public async Task StartAsync_GracefulShutdown_OnEOF_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            for (var i = 1; i <= 3; i++)
            {
                var message = $"id: {i}\ndata: Message {i}\n\n";
                await context.Response.WriteAsync(message, context.RequestAborted);
                await Task.Delay(10, context.RequestAborted);
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnMessage((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await serverSentEventsManager.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsAsyncEnumerable_GracefulShutdown_OnDoneMessage_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            for (var i = 1; i <= 3; i++)
            {
                var message = $"id: {i}\ndata: Message {i}\n\n";
                await context.Response.WriteAsync(message, context.RequestAborted);
                await Task.Delay(10, context.RequestAborted);
            }

            await context.Response.WriteAsync("data: [DONE]\n\n", context.RequestAborted);

            try { await Task.Delay(1000, context.RequestAborted); }
            catch { }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        await foreach (var data in
                       serverSentEventsManager.StartAsAsyncEnumerable(TestContext.Current.CancellationToken))
        {
            i++;
        }

        Assert.Equal(3, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_GracefulShutdown_OnLargeRetry_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            await context.Response.WriteAsync("data: Message 1\n\nretry: 999999\n\n", context.RequestAborted);
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var i = 0;
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder =
            new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"))
                .SetOnMessage((_, _) =>
                {
                    i++;
                    return Task.CompletedTask;
                });
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        // 【核心验证】：遇到极大 retry 值时，应正常接收完前面的数据，然后优雅退出，不抛异常，不重连。
        await serverSentEventsManager.StartAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, i);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartCoreAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers["X-Accel-Buffering"] = "no";

            for (var i = 1; i <= 5; i++)
            {
                var message = $"id: {i}\ndata: Message {i}\n\n";
                await context.Response.WriteAsync(message, context.RequestAborted);
                await Task.Delay(10, context.RequestAborted);
            }
        });

        await app.StartAsync(TestContext.Current.CancellationToken);

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri($"http://localhost:{port}/test"));
        var serverSentEventsManager = new ServerSentEventsManager(httpRemoteService, httpServerSentEventsBuilder);

        var messageChannel = Channel.CreateUnbounded<ServerSentEventsData>();
        var producerTask =
            serverSentEventsManager.StartCoreAsync(messageChannel.Writer, TestContext.Current.CancellationToken);

        var received = new List<ServerSentEventsData>();
        await foreach (var data in messageChannel.Reader.ReadAllAsync(TestContext.Current.CancellationToken))
        {
            received.Add(data);
        }

        await producerTask;

        Assert.Equal(5, received.Count);

        await app.StopAsync(TestContext.Current.CancellationToken);
        await serviceProvider.DisposeAsync();
    }
}