// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServiceMethodsTests
{
    [Fact]
    public async Task Send_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage = httpRemoteService.Send(httpRequestBuilder);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var _ = httpRemoteService.Send(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpResponseMessage =
            httpRemoteService.Send(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var _ = httpRemoteService.Send(httpRequestBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var httpResponseMessage = await httpRemoteService.SendAsync(httpRequestBuilder);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var _ = await httpRemoteService.SendAsync(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var httpResponseMessage =
            await httpRemoteService.SendAsync(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

        Assert.NotNull(httpResponseMessage);
        Assert.True(httpResponseMessage.IsSuccessStatusCode);
        Assert.Equal(200, (int)httpResponseMessage.StatusCode);
        Assert.Equal("Hello World!", await httpResponseMessage.Content.ReadAsStringAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var _ = await httpRemoteService.SendAsync(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_WithResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult = httpRemoteService.Send<string>(httpRequestBuilder);

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
    public async Task Send_WithResult_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var _ = httpRemoteService.Send<string>(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Send_WithResult_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            httpRemoteService.Send<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

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
    public async Task Send_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var _ = httpRemoteService.Send<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var httpRemoteResult = await httpRemoteService.SendAsync<string>(httpRequestBuilder);

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
    public async Task SendAsync_WithResult_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            var _ = await httpRemoteService.SendAsync<string>(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithResult_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

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
    public async Task SendAsync_WithResult_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            var _ = await httpRemoteService.SendAsync<string>(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsync_WithResult_GetSetCookies_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        var dateNow = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero).AddYears(1);

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            context.Response.Cookies.Append("MyCookie", "furion",
                new CookieOptions
                {
                    Expires = dateNow,
                    HttpOnly = true,
                    Secure = context.Request.IsHttps,
                    Path = "/",
                    SameSite = SameSiteMode.Strict
                }
            );
        });

        await app.StartAsync();

        // 测试代码
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var httpRemoteResult =
            await httpRemoteService.SendAsync<string>(httpRequestBuilder);

        Assert.NotNull(httpRemoteResult);
        Assert.NotNull(httpRemoteResult.ResponseMessage);
        Assert.NotNull(httpRemoteResult.RawSetCookies);
        Assert.Equal("MyCookie=furion; expires=Wed, 01 Jan 2025 00:00:00 GMT; path=/; samesite=strict; httponly",
            httpRemoteResult.RawSetCookies.First());
        Assert.NotNull(httpRemoteResult.SetCookies);
        Assert.Single(httpRemoteResult.SetCookies);

        var setCookies = httpRemoteResult.SetCookies.First();
        Assert.Equal("MyCookie", setCookies.Name.ToString());
        Assert.Equal("furion", setCookies.Value.ToString());
        Assert.Equal("/", setCookies.Path.ToString());
        Assert.True(setCookies.HttpOnly);
        Assert.Equal(Microsoft.Net.Http.Headers.SameSiteMode.Strict, setCookies.SameSite);
        Assert.Equal("2025/1/1 0:00:00 +00:00", setCookies.Expires.ToString());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.SendAs<string>(httpRequestBuilder);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs<string>(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.SendAs<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result = await httpRemoteService.SendAsAsync<string>(httpRequestBuilder);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync<string>(httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result =
            await httpRemoteService.SendAsAsync<string>(httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync<string>(httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_HttpRemoteResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result = httpRemoteService.SendAs<HttpRemoteResult<string>>(httpRequestBuilder);

        Assert.Equal("Hello World!", result?.Result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_HttpRemoteResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result = await httpRemoteService.SendAsAsync<HttpRemoteResult<string>>(httpRequestBuilder);

        Assert.Equal("Hello World!", result?.Result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }


    [Fact]
    public async Task SendAs_WithType_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var result = httpRemoteService.SendAs(typeof(string), httpRequestBuilder);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithType_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs(typeof(string), httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithType_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.SendAs(typeof(string), httpRequestBuilder, HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithType_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = httpRemoteService.SendAs(typeof(string), httpRequestBuilder, HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithType_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result = await httpRemoteService.SendAsAsync(typeof(string), httpRequestBuilder);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithType_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync(typeof(string), httpRequestBuilder, cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithType_WithHttpCompletionOption_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result =
            await httpRemoteService.SendAsAsync(typeof(string), httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithType_WithHttpCompletionOption_WithCancellationToken_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await httpRemoteService.SendAsAsync(typeof(string), httpRequestBuilder,
                HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAs_WithType_HttpRemoteResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        // ReSharper disable once MethodHasAsyncOverload
        var result =
            httpRemoteService.SendAs(typeof(HttpRemoteResult<string>), httpRequestBuilder) as HttpRemoteResult<string>;

        Assert.Equal("Hello World!", result?.Result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task SendAsAsync_WithType_HttpRemoteResult_ReturnOK()
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
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"));

        var result =
            await httpRemoteService.SendAsAsync(typeof(HttpRemoteResult<string>), httpRequestBuilder) as
                HttpRemoteResult<string>;

        Assert.Equal("Hello World!", result?.Result);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}