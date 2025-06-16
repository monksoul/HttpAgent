// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class DeclarativeManagerTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new DeclarativeManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new DeclarativeManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.Method1))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService, new HttpDeclarativeBuilder(method, []));

        Assert.NotNull(declarativeManager._httpDeclarativeBuilder);
        Assert.NotNull(declarativeManager._httpRemoteService);
        Assert.NotNull(declarativeManager.RequestBuilder);

        serviceProvider.Dispose();
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
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        // ReSharper disable once MethodHasAsyncOverload
        var result = declarativeManager.Start();
        Assert.Equal("Hello World!", result);

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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrl))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", cancellationTokenSource.Token]));

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            _ = declarativeManager.Start();
        });

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
            await Task.Delay(50);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        var result = await declarativeManager.StartAsync<string>();
        Assert.Equal("Hello World!", result);

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
            await Task.Delay(200);

            await context.Response.WriteAsync("Hello World!");
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(100);

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", cancellationTokenSource.Token]));

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            _ = await declarativeManager.StartAsync<string>();
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public void ExtractSingleSpecialArguments_Invalid_Parameters()
    {
        Assert.Throws<InvalidOperationException>(() =>
            DeclarativeManager.ExtractSingleSpecialArguments([
                HttpCompletionOption.ResponseContentRead, HttpCompletionOption.ResponseHeadersRead
            ]));

        Assert.Throws<InvalidOperationException>(() =>
            DeclarativeManager.ExtractSingleSpecialArguments([
                CancellationToken.None, CancellationToken.None
            ]));
    }

    [Fact]
    public void ExtractSingleSpecialArguments_ReturnOK()
    {
        var (completionOption1, cancellationToken1) = DeclarativeManager.ExtractSingleSpecialArguments([]);
        Assert.Equal(HttpCompletionOption.ResponseContentRead, completionOption1);
        Assert.Equal(CancellationToken.None, cancellationToken1);

        var (completionOption2, cancellationToken2) =
            DeclarativeManager.ExtractSingleSpecialArguments([HttpCompletionOption.ResponseHeadersRead]);
        Assert.Equal(HttpCompletionOption.ResponseHeadersRead, completionOption2);
        Assert.Equal(CancellationToken.None, cancellationToken2);

        using var cancellationTokenSource = new CancellationTokenSource();
        var (completionOption3, cancellationToken3) =
            DeclarativeManager.ExtractSingleSpecialArguments([
                HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token
            ]);
        Assert.Equal(HttpCompletionOption.ResponseHeadersRead, completionOption3);
        Assert.True(cancellationToken3 != CancellationToken.None);
    }

    [Fact]
    public async Task StartAsync_WithClayContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        var result = await declarativeManager.StartAsync<Clay>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.ToJsonString());
        Assert.Equal(1, result["id"]);
        Assert.Equal(1, result["Id"]);
        Assert.Equal("Furion", result["name"]);
        Assert.Equal("Furion", result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithHttpRemoteResult_And_ClayContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        var result = await declarativeManager.StartAsync<HttpRemoteResult<Clay>>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.Result!.ToJsonString());
        Assert.Equal(1, result.Result["id"]);
        Assert.Equal(1, result.Result["Id"]);
        Assert.Equal("Furion", result.Result["name"]);
        Assert.Equal("Furion", result.Result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithDynamicContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        var result = await declarativeManager.StartAsync<dynamic>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.ToJsonString());
        Assert.Equal(1, result["id"]);
        Assert.Equal(1, result["Id"]);
        Assert.Equal("Furion", result["name"]);
        Assert.Equal("Furion", result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithHttpRemoteResult_And_DynamicContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        var result = await declarativeManager.StartAsync<HttpRemoteResult<dynamic>>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.Result!.ToJsonString());
        Assert.Equal(1, result.Result["id"]);
        Assert.Equal(1, result.Result["Id"]);
        Assert.Equal("Furion", result.Result["name"]);
        Assert.Equal("Furion", result.Result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithObjectContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        dynamic? result = await declarativeManager.StartAsync<object>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.ToJsonString());
        Assert.Equal(1, result["id"]);
        Assert.Equal(1, result["Id"]);
        Assert.Equal("Furion", result["name"]);
        Assert.Equal("Furion", result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_WithHttpRemoteResult_And_ObjectContentConverter_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async context =>
        {
            await Task.Delay(50);

            await context.Response.WriteAsJsonAsync(new { id = 1, name = "Furion" });
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var method = typeof(IHttpDeclarativeTest).GetMethod(nameof(IHttpDeclarativeTest.GetUrlAsync))!;
        var declarativeManager = new DeclarativeManager(httpRemoteService,
            new HttpDeclarativeBuilder(method, [$"http://localhost:{port}/test", CancellationToken.None]));

        dynamic? result = await declarativeManager.StartAsync<HttpRemoteResult<object>>();
        Assert.Equal("{\"id\":1,\"name\":\"Furion\"}", result!.Result!.ToJsonString());
        Assert.Equal(1, result.Result["id"]);
        Assert.Equal(1, result.Result["Id"]);
        Assert.Equal("Furion", result.Result["name"]);
        Assert.Equal("Furion", result.Result["Name"]);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}