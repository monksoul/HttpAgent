// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteServiceHttpMethodsTests2
{
    [Fact]
    public async Task GetAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.GetAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.GetAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.GetAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.GetAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.GetAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.GetAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.GetAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.GetAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.GetAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.GetAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task GetAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.GetAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.GetAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.PutAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.PutAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.PutAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.PutAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.PutAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.PutAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.PutAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.PutAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.PutAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.PutAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PutAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.PutAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.PutAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.PostAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.PostAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.PostAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.PostAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.PostAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.PostAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.PostAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.PostAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.PostAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.PostAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PostAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.PostAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.PostAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.DeleteAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.DeleteAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.DeleteAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.DeleteAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.DeleteAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.DeleteAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.DeleteAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.DeleteAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.DeleteAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.DeleteAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.DeleteAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.DeleteAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.HeadAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.HeadAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("", str);
        Assert.Equal("", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.HeadAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.HeadAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("", await streamReader.ReadToEndAsync());
        Assert.Equal("", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.HeadAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.HeadAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.HeadAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.HeadAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("", str);
        Assert.Equal("", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.HeadAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.HeadAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("", await streamReader.ReadToEndAsync());
        Assert.Equal("", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task HeadAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.HeadAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.HeadAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.OptionsAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.OptionsAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.OptionsAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.OptionsAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.OptionsAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.OptionsAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.OptionsAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.OptionsAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.OptionsAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.OptionsAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task OptionsAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.OptionsAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.OptionsAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.TraceAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.TraceAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.TraceAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.TraceAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.TraceAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.TraceAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.TraceAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.TraceAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.TraceAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.TraceAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task TraceAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.TraceAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.TraceAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.PatchAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.PatchAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.PatchAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.PatchAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.PatchAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.PatchAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.PatchAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.PatchAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.PatchAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.PatchAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task PatchAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.PatchAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.PatchAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsString_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var str = httpRemoteService.QueryAsString($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var str2 = httpRemoteService.QueryAsString($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsStream_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream = httpRemoteService.QueryAsStream($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        // ReSharper disable once MethodHasAsyncOverload
        await using var stream2 = httpRemoteService.QueryAsStream($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsByteArray_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        // ReSharper disable once MethodHasAsyncOverload
        var bytes = httpRemoteService.QueryAsByteArray($"http://localhost:{port}/test");

        // ReSharper disable once MethodHasAsyncOverload
        var bytes2 = httpRemoteService.QueryAsByteArray($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsStringAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var str = await httpRemoteService.QueryAsStringAsync($"http://localhost:{port}/test");
        var str2 = await httpRemoteService.QueryAsStringAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsStreamAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        await using var stream = await httpRemoteService.QueryAsStreamAsync($"http://localhost:{port}/test");
        using var streamReader = new StreamReader(stream!);

        await using var stream2 = await httpRemoteService.QueryAsStreamAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);
        using var streamReader2 = new StreamReader(stream2!);

        Assert.Equal("Hello World!", await streamReader.ReadToEndAsync());
        Assert.Equal("Hello World!", await streamReader2.ReadToEndAsync());

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task QueryAsByteArrayAsync_ReturnOK()
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

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        var bytes = await httpRemoteService.QueryAsByteArrayAsync($"http://localhost:{port}/test");
        var bytes2 = await httpRemoteService.QueryAsByteArrayAsync($"http://localhost:{port}/test",
            HttpCompletionOption.ResponseContentRead);

        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes!));
        Assert.Equal("Hello World!", Encoding.UTF8.GetString(bytes2!));

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}