// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServiceHttpMethodsTests
{
    [Fact]
    public async Task Get_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Get($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Get($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Get($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Get($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead,
                null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.GetAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.GetAsync($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.GetAsync($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.GetAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Get<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Get<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Get<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Get_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Get<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead,
                null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.GetAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.GetAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.GetAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.GetAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.GetAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.GetAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.GetAs<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.GetAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsyncAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.GetAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsyncAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.GetAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsyncAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.GetAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsyncAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.GetAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Put($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Put($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Put($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Put($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.PutAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.PutAsync($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.PutAsync($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PutAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Put<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Put<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Put<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Put_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Put<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead,
                null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.PutAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PutAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.PutAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PutAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.PutAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.PutAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.PutAs<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.PutAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.PutAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PutAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.PutAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPut("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PutAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Post($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Post($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Post($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Post($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.PostAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.PostAsync($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.PostAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PostAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Post<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Post<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Post<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Post_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Post<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.PostAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PostAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.PostAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PostAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.PostAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.PostAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.PostAs<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.PostAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.PostAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PostAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.PostAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPost("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PostAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Delete($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Delete($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Delete($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Delete($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead,
                null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.DeleteAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.DeleteAsync($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.DeleteAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.DeleteAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Delete<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Delete<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Delete<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Delete_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Delete<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.DeleteAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.DeleteAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.DeleteAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.DeleteAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.DeleteAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.DeleteAs<string>($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.DeleteAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.DeleteAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.DeleteAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.DeleteAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.DeleteAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapDelete("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.DeleteAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Head($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Empty(await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Head($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Head($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Empty(await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Head($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.HeadAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Empty(await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.HeadAsync($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.HeadAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Empty(await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.HeadAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Head<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(0, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Empty(httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Head<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Head<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(0, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Empty(httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Head_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Head<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.HeadAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(0, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Empty(httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.HeadAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.HeadAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(0, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.NotNull(httpRemoteResult.Result);
        Assert.Empty(httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.HeadAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.HeadAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Empty(result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.HeadAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.HeadAs<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Empty(result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.HeadAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.HeadAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.HeadAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.HeadAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Empty(result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Head], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.HeadAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Options($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Options($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Options($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Options($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead,
                null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.OptionsAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.OptionsAsync($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.OptionsAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.OptionsAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Options<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Options<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Options<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Options_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Options<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.OptionsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.OptionsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.OptionsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.OptionsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.OptionsAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.OptionsAs<string>($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.OptionsAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.OptionsAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.OptionsAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.OptionsAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.OptionsAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Options], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.OptionsAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Trace($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Trace($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Trace($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Trace($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.TraceAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.TraceAsync($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.TraceAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.TraceAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Trace<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Trace<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Trace<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Trace_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Trace<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.TraceAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.TraceAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.TraceAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.TraceAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.TraceAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.TraceAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.TraceAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.TraceAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.TraceAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.TraceAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.TraceAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", [HttpMethods.Trace], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.TraceAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Patch($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Patch($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Patch($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Patch($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.PatchAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.PatchAsync($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.PatchAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PatchAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Patch<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Patch<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Patch<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Patch_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Patch<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.PatchAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PatchAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.PatchAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PatchAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.PatchAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.PatchAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.PatchAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.PatchAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.PatchAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.PatchAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.PatchAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapPatch("/test", async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.PatchAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Query($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Query($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Query($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Query($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage = await httpRemoteService.QueryAsync($"http://localhost:{port}/test");

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ =
                await httpRemoteService.QueryAsync($"http://localhost:{port}/test", null,
                    cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpResponseMessage =
            await httpRemoteService.QueryAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.QueryAsync($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Query<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.Query<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Query<string>($"http://localhost:{port}/test", HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Query_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.Query<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithResult_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var httpRemoteResult = await httpRemoteService.QueryAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithResult_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.QueryAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithResult_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.QueryAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.Equal("text/plain", httpRemoteResult.ContentType);
        Assert.Equal("utf-8", httpRemoteResult.CharSet);
        Assert.Equal(12, httpRemoteResult.ContentLength);
        Assert.Null(httpRemoteResult.RawSetCookies);
        Assert.Null(httpRemoteResult.SetCookies);
        Assert.True(httpRemoteResult.IsSuccessStatusCode);

        Assert.Equal(200, (int)httpRemoteResult.StatusCode);
        Assert.Equal("Hello World!", httpRemoteResult.Result);
        Assert.True(httpRemoteResult.RequestDuration > 0);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.QueryAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAs_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.QueryAs<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAs_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ =
                httpRemoteService.QueryAs<string>($"http://localhost:{port}/test", null, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAs_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.QueryAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.QueryAs<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsAsync_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var result = await httpRemoteService.QueryAsAsync<string>($"http://localhost:{port}/test");

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsAsync_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.QueryAsAsync<string>($"http://localhost:{port}/test", null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsAsync_WithHttpCompletionOption_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(50);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            await httpRemoteService.QueryAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(result);
        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapMethods("/test", ["QUERY"], async () =>
        {
            await Task.Delay(200);
            return "Hello World!";
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = await httpRemoteService.QueryAsAsync<string>($"http://localhost:{port}/test",
                HttpCompletionOption.ResponseContentRead, null,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}