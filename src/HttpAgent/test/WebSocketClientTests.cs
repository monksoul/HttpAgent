// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class WebSocketClientTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new WebSocketClient((string)null!));
        Assert.Throws<ArgumentException>(() => new WebSocketClient(string.Empty));
        Assert.Throws<ArgumentException>(() => new WebSocketClient(" "));

        Assert.Throws<ArgumentNullException>(() => new WebSocketClient((Uri)null!));
        Assert.Throws<ArgumentNullException>(() => new WebSocketClient((WebSocketClientOptions)null!));

        Assert.Throws<ArgumentNullException>(() => new WebSocketClient("ws://localhost:12345", null!));
        Assert.Throws<ArgumentNullException>(() => new WebSocketClient(new Uri("ws://localhost:12345"), null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        using var webSocketClient = new WebSocketClient("ws://localhost:12345");
        Assert.NotNull(webSocketClient.Options);
        Assert.Equal("ws://localhost:12345/", webSocketClient.Options.ServerUri.ToString());

        using var webSocketClient2 = new WebSocketClient(new Uri("ws://localhost:12345"));
        Assert.NotNull(webSocketClient2.Options);
        Assert.Equal("ws://localhost:12345/", webSocketClient2.Options.ServerUri.ToString());

        using var webSocketClient3 = new WebSocketClient(new WebSocketClientOptions("ws://localhost:12345"));
        Assert.NotNull(webSocketClient3.Options);
        Assert.Equal("ws://localhost:12345/", webSocketClient3.Options.ServerUri.ToString());
        Assert.Null(webSocketClient3._clientWebSocket);
        Assert.Null(webSocketClient3._messageCancellationTokenSource);
        Assert.Null(webSocketClient3._receiveMessageTask);
        Assert.Null(webSocketClient3.State);
        Assert.Equal(0, webSocketClient3.CurrentReconnectRetries);

        using var webSocketClient4 =
            new WebSocketClient("ws://localhost:12345", o => o.SetRequestHeader("cookie", "furion"));
        Assert.NotNull(webSocketClient4.Options);
        Assert.Equal("ws://localhost:12345/", webSocketClient4.Options.ServerUri.ToString());

        using var webSocketClient5 =
            new WebSocketClient(new Uri("ws://localhost:12345"), o => o.SetRequestHeader("cookie", "furion"));
        Assert.NotNull(webSocketClient5.Options);
        Assert.Equal("ws://localhost:12345/", webSocketClient5.Options.ServerUri.ToString());
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var webSocketClient = new WebSocketClient("ws://localhost:12345");
        webSocketClient.Dispose();

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);
    }

    [Fact]
    public async Task ConnectAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        await webSocketClient.ConnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await Task.Delay(200);
        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_ConnectEvents_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.Connecting += (s, e) =>
        {
            i++;
        };
        webSocketClient.Connected += (s, e) =>
        {
            i++;
        };
        await webSocketClient.ConnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(2, i);

        await Task.Delay(200);
        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await Task.Delay(150);
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await webSocketClient.ConnectAsync(cancellationTokenSource.Token));

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Closed, webSocketClient.State);

        await webSocketClient.CloseAsync(cancellationTokenSource.Token);
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_WithTimeout_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await Task.Delay(150);
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient(new WebSocketClientOptions($"ws://localhost:{port}/ws")
        {
            Timeout = TimeSpan.FromMilliseconds(100)
        });

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await webSocketClient.ConnectAsync());

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Closed, webSocketClient.State);

        await webSocketClient.CloseAsync();

        using var webSocketClient2 =
            new WebSocketClient(new WebSocketClientOptions($"ws://localhost:{port}/ws") { Timeout = TimeSpan.Zero });
        await webSocketClient2.ConnectAsync();
        Assert.NotNull(webSocketClient2._clientWebSocket);
        Assert.NotNull(webSocketClient2.State);
        Assert.Equal(WebSocketState.Open, webSocketClient2.State);

        await webSocketClient2.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_ConfigureClientWebSocketOptions_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        var i = 0;

        var options = new WebSocketClientOptions($"ws://localhost:{port}/ws")
        {
            Configure = o =>
            {
                i++;
            }
        };
        using var webSocketClient = new WebSocketClient(options);

        await webSocketClient.ConnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(1, i);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_Duplicate_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        var i = 0;
        webSocketClient.Connecting += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
        await webSocketClient.ConnectAsync();
        await webSocketClient.ConnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);
        Assert.Equal(1, i);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ConnectAsync_Reconnect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    throw new Exception("出错了");
#pragma warning disable CS0162 // 检测到不可到达的代码
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
#pragma warning restore CS0162 // 检测到不可到达的代码
                    await Echo(webSocket);
                }

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        var options = new WebSocketClientOptions($"ws://localhost:{port}/ws")
        {
            MaxReconnectRetries = 5, ReconnectInterval = TimeSpan.FromMilliseconds(100)
        };
        using var webSocketClient = new WebSocketClient(options);
        var i = 0;
        webSocketClient.Reconnecting += (s, e) =>
        {
            i++;
        };

        await Assert.ThrowsAsync<WebSocketException>(async () => await webSocketClient.ConnectAsync());

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient.State);
        Assert.Equal(5, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ReconnectAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        await webSocketClient.ReconnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ReconnectAsync_Duplicate_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        await webSocketClient.ReconnectAsync();
        await webSocketClient.ReconnectAsync();

        Assert.NotNull(webSocketClient._clientWebSocket);
        Assert.NotNull(webSocketClient.State);
        Assert.Equal(WebSocketState.Open, webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task CloseAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        await webSocketClient.ConnectAsync();
        await webSocketClient.CloseAsync();
        await webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing");

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await app.StopAsync();
    }

    [Fact]
    public async Task CloseAsync_Events_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.Closing += (s, e) =>
        {
            i++;
        };
        webSocketClient.Closed += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
        await webSocketClient.CloseAsync();

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);
        Assert.Equal(2, i);

        await app.StopAsync();
    }

    [Fact]
    public async Task CloseAsync_Duplicate_Events_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.Closing += (s, e) =>
        {
            i++;
        };
        webSocketClient.Closed += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
        await webSocketClient.CloseAsync();
        await webSocketClient.CloseAsync();
        await webSocketClient.CloseAsync();

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);
        Assert.Equal(2, i);

        await app.StopAsync();
    }

    [Fact]
    public async Task ListenAsync_NoConnect_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");
        await webSocketClient.ListenAsync();

        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task ListenAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.TextReceived += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
        await webSocketClient.ListenAsync();
        await webSocketClient.SendAsync("ok");
        await webSocketClient.CloseAsync();

        Assert.Equal(1, i);
        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task WaitAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.TextReceived += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        Task.Run(async () => await webSocketClient.WaitAsync());
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        await webSocketClient.SendAsync("ok");

        await webSocketClient.CloseAsync();

        Assert.Equal(1, i);
        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    [Fact]
    public async Task SendAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();
        app.UseWebSockets();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(webSocket);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                await next(context);
            }
        });

        await app.StartAsync();

        using var webSocketClient = new WebSocketClient($"ws://localhost:{port}/ws");

        var i = 0;
        webSocketClient.TextReceived += (s, e) =>
        {
            i++;
        };
        webSocketClient.BinaryReceived += (s, e) =>
        {
            i++;
        };

        await webSocketClient.ConnectAsync();
        await webSocketClient.ListenAsync();
        await webSocketClient.SendAsync("ok");
        await webSocketClient.SendAsync("ok2", WebSocketMessageType.Text);
        await webSocketClient.SendAsync("Furion"u8.ToArray());
        await Task.Delay(1000);
        await webSocketClient.CloseAsync();

        Assert.Equal(3, i);
        Assert.Null(webSocketClient._clientWebSocket);
        Assert.Null(webSocketClient._messageCancellationTokenSource);
        Assert.Null(webSocketClient._receiveMessageTask);
        Assert.Null(webSocketClient.State);
        Assert.Equal(0, webSocketClient.CurrentReconnectRetries);

        await webSocketClient.CloseAsync();
        await app.StopAsync();
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            if (receiveResult.MessageType == WebSocketMessageType.Text)
            {
                var text = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                Console.WriteLine(text);
            }

            if (receiveResult.MessageType == WebSocketMessageType.Binary)
            {
                var bytes = new byte[receiveResult.Count];
                Buffer.BlockCopy(buffer, 0, bytes, 0, receiveResult.Count);
                var text = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Console.WriteLine(text);
            }

            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}