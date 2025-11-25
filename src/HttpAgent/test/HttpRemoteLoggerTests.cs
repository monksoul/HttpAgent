// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteLoggerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<HttpRemoteOptions>(_ => { });
        using var serviceProvider = services.BuildServiceProvider();

        var logger = new HttpRemoteLogger(serviceProvider.GetRequiredService<ILogger<Logging>>(),
            serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>(), false);
        Assert.NotNull(logger);
    }

    [Fact]
    public void Log_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<HttpRemoteOptions>(_ => { });
        using var serviceProvider = services.BuildServiceProvider();

        var logger = new HttpRemoteLogger(serviceProvider.GetRequiredService<ILogger<Logging>>(),
            serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>(), false);

        logger.LogInformation("Message");
        logger.LogTrace("Message");
        logger.LogDebug("Message");
        logger.LogWarning("Message");
        logger.LogCritical("Message");
        logger.LogError(new Exception("错误了"), "Message");
        logger.Log(LogLevel.Information, null, "错误消息");
    }

    [Fact]
    public void Log_WithLogging_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.Configure<HttpRemoteOptions>(_ => { });
        using var serviceProvider = services.BuildServiceProvider();

        var logger = new HttpRemoteLogger(serviceProvider.GetRequiredService<ILogger<Logging>>(),
            serviceProvider.GetRequiredService<IOptions<HttpRemoteOptions>>(), true);

        logger.LogInformation("Message");
        logger.LogTrace("Message");
        logger.LogDebug("Message");
        logger.LogWarning("Message");
        logger.LogCritical("Message");
        logger.LogError(new Exception("错误了"), "Message");
        logger.Log(LogLevel.Information, null, "错误消息");
    }
}