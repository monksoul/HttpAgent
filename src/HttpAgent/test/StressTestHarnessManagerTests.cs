// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class StressTestHarnessManagerTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();

        Assert.Throws<ArgumentNullException>(() => new StressTestHarnessManager(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new StressTestHarnessManager(httpRemoteService, null!));

        serviceProvider.Dispose();
    }

    [Fact]
    public void New_ReturnOK()
    {
        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService,
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost:5000")));

        Assert.NotNull(stressTestHarnessManager._httpStressTestHarnessBuilder);
        Assert.NotNull(stressTestHarnessManager._httpRemoteService);
        Assert.NotNull(stressTestHarnessManager.RequestBuilder);

        var stressTestHarnessManager2 = new StressTestHarnessManager(httpRemoteService,
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri("http://localhost:5000")).WithRequest(builder =>
                builder.SetTimeout(100)));
        Assert.NotNull(stressTestHarnessManager2.RequestBuilder);
        Assert.Equal(TimeSpan.FromMilliseconds(100), stressTestHarnessManager2.RequestBuilder.Timeout);

        serviceProvider.Dispose();
    }

    [Fact]
    public async Task Start_ReturnOK()
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
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var result = stressTestHarnessManager.Start();
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task Start_FailRequest_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            throw new Exception("出错了");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello Furion";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        // ReSharper disable once MethodHasAsyncOverload
        var result = stressTestHarnessManager.Start();
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(0, result.SuccessfulRequests);
        Assert.Equal(10, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

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
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        Assert.Throws<TaskCanceledException>(() =>
        {
            // ReSharper disable once MethodHasAsyncOverload
            stressTestHarnessManager.Start(HttpCompletionOption.ResponseContentRead, cancellationTokenSource.Token);
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
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        var result = await stressTestHarnessManager.StartAsync();
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(10, result.SuccessfulRequests);
        Assert.Equal(0, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 15);

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }

    [Fact]
    public async Task StartAsync_FailRequest_ReturnOK()
    {
        var port = NetworkUtility.FindAvailableTcpPort();
        var urls = new[] { "--urls", $"http://localhost:{port}" };
        var builder = WebApplication.CreateBuilder(urls);
        await using var app = builder.Build();

        app.MapGet("/test", async () =>
        {
            await Task.Delay(50);
            throw new Exception("出错了");
#pragma warning disable CS0162 // 检测到不可到达的代码
            return "Hello Furion";
#pragma warning restore CS0162 // 检测到不可到达的代码
        });

        await app.StartAsync();

        var (httpRemoteService, serviceProvider) = Helpers.CreateHttpRemoteService();
        var httpStressTestHarnessBuilder =
            new HttpStressTestHarnessBuilder(HttpMethod.Get, new Uri($"http://localhost:{port}/test"))
                .SetNumberOfRequests(10);
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        var result = await stressTestHarnessManager.StartAsync();
        Assert.NotNull(result);

        Assert.Equal(10, result.TotalRequests);
        Assert.Equal(0, result.SuccessfulRequests);
        Assert.Equal(10, result.FailedRequests);
        Assert.True(result.QueriesPerSecond > 50);

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
        var stressTestHarnessManager = new StressTestHarnessManager(httpRemoteService, httpStressTestHarnessBuilder);

        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(50);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await stressTestHarnessManager.StartAsync(HttpCompletionOption.ResponseContentRead,
                cancellationTokenSource.Token);
        });

        await app.StopAsync();
        await serviceProvider.DisposeAsync();
    }
}