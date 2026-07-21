// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class SuppressExceptionPipelineHandlerTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<HttpRemoteOptions>();
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, true));

        using var serviceProvider = services.BuildServiceProvider();

        var handler = new SuppressExceptionPipelineHandler(serviceProvider.GetRequiredService<IHttpRemoteLogger>());
        Assert.NotNull(handler);
    }

    [Fact]
    public void ShouldSuppressException_ReturnOK()
    {
        Assert.False(SuppressExceptionPipelineHandler.ShouldSuppressException(null, null));
        Assert.False(SuppressExceptionPipelineHandler.ShouldSuppressException([], null));
        Assert.False(SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(Exception)], null));

        Assert.True(SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(Exception)], new Exception()));
        Assert.True(
            SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(Exception)], new InvalidCastException()));
        Assert.True(
            SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(Exception)], new TimeoutException()));
        Assert.True(
            SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(Exception)], new HttpRequestException()));
        Assert.False(
            SuppressExceptionPipelineHandler.ShouldSuppressException([typeof(TimeoutException)], new Exception()));
    }
}