// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class RetryPipelineHandlerTests
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

        var handler = new RetryPipelineHandler(serviceProvider.GetRequiredService<IHttpRemoteLogger>());
        Assert.NotNull(handler);
    }

    [Fact]
    public void GetRetryDelay_Invalid_Parameter() =>
        Assert.Throws<ArgumentNullException>(() => RetryPipelineHandler.GetRetryDelay(null!, 0));

    [Fact]
    public void GetRetryDelay_ReturnOK()
    {
        var defaultOptions = new HttpRetryOptions();
        Assert.Equal(TimeSpan.FromSeconds(1), RetryPipelineHandler.GetRetryDelay(defaultOptions, 0));
        Assert.Equal(TimeSpan.FromSeconds(1), RetryPipelineHandler.GetRetryDelay(defaultOptions, 3));

        var customIntervalOptions = new HttpRetryOptions().SetRetryInterval(TimeSpan.FromSeconds(5));
        Assert.Equal(TimeSpan.FromSeconds(5), RetryPipelineHandler.GetRetryDelay(customIntervalOptions, 0));
        Assert.Equal(TimeSpan.FromSeconds(5), RetryPipelineHandler.GetRetryDelay(customIntervalOptions, 2));

        var exponentialOptions = new HttpRetryOptions().SetUseExponentialBackoff(true);
        Assert.Equal(TimeSpan.FromSeconds(1), RetryPipelineHandler.GetRetryDelay(exponentialOptions, 0));
        Assert.Equal(TimeSpan.FromSeconds(2), RetryPipelineHandler.GetRetryDelay(exponentialOptions, 1));
        Assert.Equal(TimeSpan.FromSeconds(4), RetryPipelineHandler.GetRetryDelay(exponentialOptions, 2));
        Assert.Equal(TimeSpan.FromSeconds(8), RetryPipelineHandler.GetRetryDelay(exponentialOptions, 3));

        var exponentialCustomOptions = new HttpRetryOptions()
            .SetUseExponentialBackoff(true)
            .SetRetryInterval(TimeSpan.FromSeconds(3));
        Assert.Equal(TimeSpan.FromSeconds(3), RetryPipelineHandler.GetRetryDelay(exponentialCustomOptions, 0));
        Assert.Equal(TimeSpan.FromSeconds(6), RetryPipelineHandler.GetRetryDelay(exponentialCustomOptions, 1));
        Assert.Equal(TimeSpan.FromSeconds(12), RetryPipelineHandler.GetRetryDelay(exponentialCustomOptions, 2));

        var intervalArrayOptions = new HttpRetryOptions()
            .SetRetryIntervals(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
        Assert.Equal(TimeSpan.FromSeconds(2), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 0));
        Assert.Equal(TimeSpan.FromSeconds(5), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 1));
        Assert.Equal(TimeSpan.FromSeconds(10), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 2));
        Assert.Equal(TimeSpan.FromSeconds(2), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 3));
        Assert.Equal(TimeSpan.FromSeconds(5), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 4));
        Assert.Equal(TimeSpan.FromSeconds(10), RetryPipelineHandler.GetRetryDelay(intervalArrayOptions, 5));

        var msArrayOptions = new HttpRetryOptions().SetRetryIntervals(500d, 1000d, 2000d);
        Assert.Equal(TimeSpan.FromMilliseconds(500), RetryPipelineHandler.GetRetryDelay(msArrayOptions, 0));
        Assert.Equal(TimeSpan.FromMilliseconds(1000), RetryPipelineHandler.GetRetryDelay(msArrayOptions, 1));
        Assert.Equal(TimeSpan.FromMilliseconds(2000), RetryPipelineHandler.GetRetryDelay(msArrayOptions, 2));
        Assert.Equal(TimeSpan.FromMilliseconds(500), RetryPipelineHandler.GetRetryDelay(msArrayOptions, 3));

        var negativeIntervalOptions = new HttpRetryOptions { RetryInterval = TimeSpan.FromSeconds(-1) };
        var negativeDelay = RetryPipelineHandler.GetRetryDelay(negativeIntervalOptions, 0);
        Assert.True(negativeDelay < TimeSpan.Zero);

        var emptyArrayOptions = new HttpRetryOptions { RetryIntervals = new List<TimeSpan>() };
        Assert.Equal(TimeSpan.FromSeconds(1), RetryPipelineHandler.GetRetryDelay(emptyArrayOptions, 0)); // 回退到默认

        var largeAttemptOptions = new HttpRetryOptions().SetUseExponentialBackoff(true);
        var largeDelay = RetryPipelineHandler.GetRetryDelay(largeAttemptOptions, 30);
        Assert.True(largeDelay > TimeSpan.FromDays(1000)); // 极大值

        var msIntervalOptions = new HttpRetryOptions().SetRetryInterval(1500d);
        Assert.Equal(TimeSpan.FromMilliseconds(1500), RetryPipelineHandler.GetRetryDelay(msIntervalOptions, 0));
    }

    [Fact]
    public void ShouldRetryOnException_ReturnOK()
    {
        Assert.True(RetryPipelineHandler.ShouldRetryOnException(null, new Exception()));
        Assert.True(RetryPipelineHandler.ShouldRetryOnException([], new Exception()));
        Assert.True(RetryPipelineHandler.ShouldRetryOnException([typeof(Exception)], new Exception()));
        Assert.True(RetryPipelineHandler.ShouldRetryOnException([typeof(Exception)], new InvalidCastException()));
        Assert.False(RetryPipelineHandler.ShouldRetryOnException([typeof(InvalidCastException)], new Exception()));
        Assert.True(
            RetryPipelineHandler.ShouldRetryOnException([typeof(InvalidCastException)], new InvalidCastException()));
        Assert.True(RetryPipelineHandler.ShouldRetryOnException(
            [typeof(InvalidCastException), typeof(InvalidDataException)], new InvalidCastException()));
    }

    [Fact]
    public void ShouldRetryOnStatusCode_ReturnOK()
    {
        Assert.False(RetryPipelineHandler.ShouldRetryOnStatusCode(null, HttpStatusCode.InternalServerError));
        Assert.False(RetryPipelineHandler.ShouldRetryOnStatusCode([], HttpStatusCode.InternalServerError));
        Assert.True(RetryPipelineHandler.ShouldRetryOnStatusCode([HttpStatusCode.InternalServerError],
            HttpStatusCode.InternalServerError));
        Assert.False(RetryPipelineHandler.ShouldRetryOnStatusCode([HttpStatusCode.InternalServerError],
            HttpStatusCode.Unauthorized));
    }

    [Fact]
    public void DefaultOnRetry_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions<HttpRemoteOptions>();
        services.TryAddSingleton<IHttpRemoteLogger>(provider =>
            ActivatorUtilities.CreateInstance<HttpRemoteLogger>(provider, true));

        using var serviceProvider = services.BuildServiceProvider();

        var handler = new RetryPipelineHandler(serviceProvider.GetRequiredService<IHttpRemoteLogger>());

        handler.DefaultOnRetry(new HttpRetryContext
        {
            Attempt = 1, StatusCode = HttpStatusCode.InternalServerError, MaxRetries = 3
        });
    }
}