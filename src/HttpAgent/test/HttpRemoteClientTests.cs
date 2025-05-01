// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

[Collection("HttpRemoteClientTests")]
public class HttpRemoteClientTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var httpRemoteService = HttpRemoteClient.Service;
        Assert.NotNull(HttpRemoteClient._serviceProvider);
        Assert.NotNull(HttpRemoteClient._lazyService);
        Assert.NotNull(HttpRemoteClient._lazyService.Value);
        Assert.NotNull(HttpRemoteClient._lock);
        Assert.NotNull(HttpRemoteClient._configure);
        Assert.NotNull(HttpRemoteClient.Service);

        Assert.Same(httpRemoteService, HttpRemoteClient.Service);
    }

    [Fact]
    public void Configure_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => HttpRemoteClient.Configure(null!));

    [Fact]
    public void Configure_ReturnOK()
    {
        var httpRemoteService = HttpRemoteClient.Service;
        HttpRemoteClient.Configure(service =>
        {
            service.AddHttpRemote()
                .ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddProfilerDelegatingHandler());
        });
        var httpRemoteService2 = HttpRemoteClient.Service;

        Assert.NotSame(httpRemoteService, httpRemoteService2);
        Assert.Same(httpRemoteService2, HttpRemoteClient.Service);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        _ = HttpRemoteClient.Service;
        Assert.NotNull(HttpRemoteClient._serviceProvider);
        HttpRemoteClient.Dispose();
        Assert.Null(HttpRemoteClient._serviceProvider);
        Assert.Throws<ObjectDisposedException>(() => HttpRemoteClient.Service);
    }

    [Fact]
    public void CreateService_ReturnOK()
    {
        _ = HttpRemoteClient.Service;

        var httpRemoteService2 = HttpRemoteClient.CreateService();
        var httpRemoteService3 = HttpRemoteClient.CreateService();
        Assert.Same(httpRemoteService2, httpRemoteService3);
    }

    [Fact]
    public void Reinitialize_ReturnOK()
    {
        var httpRemoteService = HttpRemoteClient.Service;
        HttpRemoteClient.Reinitialize();

        var httpRemoteService2 = HttpRemoteClient.Service;
        Assert.NotSame(httpRemoteService, httpRemoteService2);
        Assert.Same(httpRemoteService2, HttpRemoteClient.Service);
    }

    [Fact]
    public void ReleaseServiceProvider_ReturnOK()
    {
        _ = HttpRemoteClient.Service;
        HttpRemoteClient.ReleaseServiceProvider();

        Assert.Null(HttpRemoteClient._serviceProvider);
    }

    [Fact]
    public async Task SimpleTest_ReturnOK()
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

        var str = await HttpRemoteClient.Service.SendAsStringAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"));
        var str2 = await HttpRemoteClient.Service.SendAsStringAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"));

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync();
        HttpRemoteClient.Dispose();
    }
}