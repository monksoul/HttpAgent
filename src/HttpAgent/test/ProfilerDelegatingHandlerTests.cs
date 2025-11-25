// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class ProfilerDelegatingHandlerTests
{
    [Fact]
    public void ProfilerDelegatingHandler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<HttpRemoteOptions>(options => { });
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();

        var handler = new ProfilerDelegatingHandler(logger, provider.GetRequiredService<IOptions<HttpRemoteOptions>>());
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task LogRequestAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        await using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();

        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        await ProfilerDelegatingHandler.LogRequestAsync(logger,
            new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning },
            httpRequestMessage);
    }

    [Fact]
    public async Task LogResponseAsync_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        await using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");

        var httpResponseMessage =
            new HttpResponseMessage { RequestMessage = httpRequestMessage, StatusCode = HttpStatusCode.OK };
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
        httpResponseMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        httpResponseMessage.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

        await ProfilerDelegatingHandler.LogResponseAsync(logger,
            new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning },
            httpResponseMessage, 200);
    }

    [Fact]
    public void Log_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ProfilerDelegatingHandler.Log(null!,
            new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning }, null!));

    [Fact]
    public void Log_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();
        var remoteOptions = new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning };

        ProfilerDelegatingHandler.Log(logger, remoteOptions, null);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, string.Empty);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, " ");
        ProfilerDelegatingHandler.Log(logger, remoteOptions, "HttpAgent.Tests");
    }

    [Fact]
    public void Log_WithHttpRemoteProfiler_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();
        var remoteOptions = new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning };

        var httpRemoteProfiler = new HttpRemoteAnalyzer();
        ProfilerDelegatingHandler.Log(logger, remoteOptions, null, httpRemoteProfiler);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, string.Empty, httpRemoteProfiler);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, " ", httpRemoteProfiler);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, "HttpAgent.Tests", httpRemoteProfiler);
        ProfilerDelegatingHandler.Log(logger, remoteOptions, "__", httpRemoteProfiler);
        Assert.Equal("HttpAgent.Tests\r\n__", httpRemoteProfiler.Data);
    }

    [Fact]
    public void IsEnabled_ReturnOK()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        Assert.True(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));

        httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(Constants.DISABLE_PROFILER_KEY), "value");
        Assert.True(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));

        httpRequestMessage.Options.Set(new HttpRequestOptionsKey<string>(Constants.DISABLE_PROFILER_KEY), "TRUE");
        Assert.False(ProfilerDelegatingHandler.IsEnabled(httpRequestMessage));
    }

    [Fact]
    public void ExtractCookieContainer_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.TryAddTransient<ProfilerDelegatingHandler>();
        services.AddHttpClient(string.Empty).AddProfilerDelegatingHandler();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));

        using var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<ProfilerDelegatingHandler>();
        Assert.Null(handler.ExtractCookieContainer());
    }

    [Fact]
    public void LogCookieContainer_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var isLoggingRegistered = services.Any(u => u.ServiceType == typeof(ILoggerProvider));
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, isLoggingRegistered));
        using var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<IHttpRemoteLogger>();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri("http://localhost"));
        var remoteOptions = new HttpRemoteOptions { ProfilerLogLevel = LogLevel.Warning };

        ProfilerDelegatingHandler.LogCookieContainer(logger, remoteOptions, httpRequestMessage, null);

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(new Uri("http://localhost"), new Cookie("cookieName", "cookieValue"));
        ProfilerDelegatingHandler.LogCookieContainer(logger, remoteOptions, httpRequestMessage, cookieContainer);
    }
}