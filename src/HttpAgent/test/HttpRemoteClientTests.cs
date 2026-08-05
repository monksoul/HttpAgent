// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

[Collection("HttpRemoteClientTests")]
public class HttpRemoteClientTests
{
    public HttpRemoteClientTests() => ResetStaticState();

    private static void ResetStaticState()
    {
        HttpRemoteClient._isDisposed = false;
        HttpRemoteClient.ReleaseInternalServiceProvider();
        HttpRemoteClient._externalServiceProvider = null;

        HttpRemoteClient._serviceInstance = null;
        HttpRemoteClient._configure = services => services.AddHttpRemote();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var httpRemoteService = HttpRemoteClient.Service;
        Assert.NotNull(HttpRemoteClient._serviceProvider);
        Assert.NotNull(HttpRemoteClient._serviceInstance);
        Assert.NotNull(HttpRemoteClient._lock);
        Assert.NotNull(HttpRemoteClient._configure);
        Assert.False(HttpRemoteClient._isDisposed);
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
    public void Configure_AutoRegisterRequiredServices_ReturnOK()
    {
        HttpRemoteClient.Configure(service =>
        {
            service.AddHttpClient();
        });

        Assert.NotNull(HttpRemoteClient.Service);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        _ = HttpRemoteClient.Service;
        Assert.NotNull(HttpRemoteClient._serviceProvider);
        HttpRemoteClient.Dispose();
        Assert.Null(HttpRemoteClient._serviceProvider);
        Assert.True(HttpRemoteClient._isDisposed);
        Assert.Throws<ObjectDisposedException>(() => HttpRemoteClient.Service);
    }

    [Fact]
    public void CreateService_ReturnOK()
    {
        _ = HttpRemoteClient.Service;

        var httpRemoteService2 = HttpRemoteClient.CreateService();
        var httpRemoteService3 = HttpRemoteClient.CreateService();
        Assert.NotSame(httpRemoteService2, httpRemoteService3);
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
    public void ReleaseInternalServiceProvider_ReturnOK()
    {
        _ = HttpRemoteClient.Service;
        HttpRemoteClient.ReleaseInternalServiceProvider();

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

        await app.StartAsync(TestContext.Current.CancellationToken);

        var str = await HttpRemoteClient.Service.SendAsStringAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"), TestContext.Current.CancellationToken);
        var str2 = await HttpRemoteClient.Service.SendAsStringAsync(
            HttpRequestBuilder.Get($"http://localhost:{port}/test"), TestContext.Current.CancellationToken);

        Assert.Equal("Hello World!", str);
        Assert.Equal("Hello World!", str2);

        await app.StopAsync(TestContext.Current.CancellationToken);
        HttpRemoteClient.Dispose();
    }

    [Fact]
    public void SetServiceProvider_ValidProvider_ServiceReturnsFromExternal_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        var provider = services.BuildServiceProvider();

        HttpRemoteClient.SetServiceProvider(provider);

        var service = HttpRemoteClient.Service;
        Assert.NotNull(service);
        Assert.Same(provider.GetRequiredService<IHttpRemoteService>(), service);
        Assert.Null(HttpRemoteClient._serviceProvider);
        Assert.Same(provider, HttpRemoteClient._externalServiceProvider);
    }

    [Fact]
    public void SetServiceProvider_MissingRegistration_ThrowsOnAccess_ReturnOK()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        HttpRemoteClient.SetServiceProvider(provider);

        Assert.Throws<InvalidOperationException>(() => HttpRemoteClient.Service);
    }

    [Fact]
    public void SetServiceProvider_OverrideExternalProvider_UsesNewProvider_ReturnOK()
    {
        var services1 = new ServiceCollection();
        services1.AddHttpRemote();
        var provider1 = services1.BuildServiceProvider();

        var services2 = new ServiceCollection();
        services2.AddHttpRemote();
        var provider2 = services2.BuildServiceProvider();

        HttpRemoteClient.SetServiceProvider(provider1);
        var firstService = HttpRemoteClient.Service;

        HttpRemoteClient.SetServiceProvider(provider2);
        var secondService = HttpRemoteClient.Service;

        Assert.NotSame(firstService, secondService);
        Assert.Same(provider2.GetRequiredService<IHttpRemoteService>(), secondService);
    }

    [Fact]
    public void Configure_AfterSetServiceProvider_DoesNotAffectExternalResolution_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        var provider = services.BuildServiceProvider();

        HttpRemoteClient.SetServiceProvider(provider);
        var serviceBefore = HttpRemoteClient.Service;

        HttpRemoteClient.Configure(sc => sc.AddHttpClient());
        var serviceAfter = HttpRemoteClient.Service;

        Assert.Same(serviceBefore, serviceAfter);
        Assert.Null(HttpRemoteClient._serviceProvider);
    }

    [Fact]
    public void Dispose_WithExternalProvider_ClearsReferenceButDoesNotDisposeExternal_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        var provider = services.BuildServiceProvider();

        HttpRemoteClient.SetServiceProvider(provider);
        var service = HttpRemoteClient.Service;
        Assert.NotNull(service);

        HttpRemoteClient.Dispose();

        Assert.Null(HttpRemoteClient._externalServiceProvider);
        Assert.Null(HttpRemoteClient._serviceInstance);
        Assert.True(HttpRemoteClient._isDisposed);

        var resolved = provider.GetRequiredService<IHttpRemoteService>();
        Assert.NotNull(resolved);
    }

    [Fact]
    public void UseHttpRemoteClient_ExtensionMethod_ReturnsSameProviderAndSetsExternal_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote();
        var provider = services.BuildServiceProvider();

        var result = provider.UseHttpRemoteClient();

        Assert.Same(provider, result);
        Assert.Same(provider, HttpRemoteClient._externalServiceProvider);
        Assert.NotNull(HttpRemoteClient.Service);
    }
}