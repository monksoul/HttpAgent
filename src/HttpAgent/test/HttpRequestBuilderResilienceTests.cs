// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRequestBuilderResilienceTests
{
    [Fact]
    public void SetRetryIndefinitely_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetRetryIndefinitely();

        Assert.NotNull(httpRequestBuilder.RetryOptions);
        Assert.Equal(TimeSpan.FromSeconds(1), httpRequestBuilder.RetryOptions.RetryInterval);
        Assert.Null(httpRequestBuilder.RetryOptions?.RetryIntervals);
    }

    [Fact]
    public void SetRetry_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.SetRetry((HttpRetryOptions)null!));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.SetRetry((Action<HttpRetryOptions>)null!));
    }

    [Fact]
    public void SetRetry_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetRetry(10);
        Assert.NotNull(httpRequestBuilder.RetryOptions);
        Assert.Equal(10, httpRequestBuilder.RetryOptions.MaxRetries);

        var httpRequestBuilder2 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder2.SetRetry(10, null);
        Assert.NotNull(httpRequestBuilder2.RetryOptions);
        Assert.Equal(10, httpRequestBuilder2.RetryOptions.MaxRetries);
        Assert.Null(httpRequestBuilder2.RetryOptions.OnRetry);
        httpRequestBuilder2.SetRetry(10, _ => { });
        Assert.NotNull(httpRequestBuilder2.RetryOptions.OnRetry);

        var httpRequestBuilder3 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder3.SetRetry(10, TimeSpan.FromSeconds(1));
        Assert.NotNull(httpRequestBuilder3.RetryOptions);
        Assert.Equal(10, httpRequestBuilder3.RetryOptions.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(1), httpRequestBuilder3.RetryOptions.RetryInterval);
        Assert.Null(httpRequestBuilder3.RetryOptions.OnRetry);
        httpRequestBuilder3.SetRetry(10, TimeSpan.FromSeconds(1), _ => { });
        Assert.NotNull(httpRequestBuilder3.RetryOptions.OnRetry);

        var httpRequestBuilder4 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder4.SetRetry(10, 1000);
        Assert.NotNull(httpRequestBuilder4.RetryOptions);
        Assert.Equal(10, httpRequestBuilder4.RetryOptions.MaxRetries);
        Assert.Equal(TimeSpan.FromSeconds(1), httpRequestBuilder4.RetryOptions.RetryInterval);
        Assert.Null(httpRequestBuilder4.RetryOptions.OnRetry);
        httpRequestBuilder4.SetRetry(10, 1000, _ => { });
        Assert.NotNull(httpRequestBuilder4.RetryOptions.OnRetry);

        var httpRequestBuilder6 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder6.SetRetry(new HttpRetryOptions());
        Assert.NotNull(httpRequestBuilder6.RetryOptions);

        var httpRequestBuilder7 = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder7.SetRetry(_ => { });
        Assert.NotNull(httpRequestBuilder7.RetryOptions);
    }

    [Fact]
    public void WithoutTimeout_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.WithoutTimeout();
        Assert.Equal(Timeout.InfiniteTimeSpan, httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);
    }

    [Fact]
    public void SetTimeout_Invalid_Parameters()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => { httpRequestBuilder.SetTimeout(-2); });

        Assert.Equal(
            "Timeout value must be greater than or equal to -1. Use -1 for infinite timeout, 0 for immediate cancellation. (Parameter 'milliseconds')",
            exception.Message);

        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.SetTimeout((HttpTimeoutOptions)null!));
        Assert.Throws<ArgumentNullException>(() => httpRequestBuilder.SetTimeout((Action<HttpTimeoutOptions>)null!));
    }

    [Fact]
    public void SetTimeout_ReturnOK()
    {
        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, new Uri("http://localhost"));
        httpRequestBuilder.SetTimeout(TimeSpan.MaxValue);
        Assert.NotNull(httpRequestBuilder.TimeoutOptions);
        Assert.Equal(TimeSpan.MaxValue, httpRequestBuilder.TimeoutOptions.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(1000);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(0);
        Assert.Equal(TimeSpan.Zero, httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(1200, () => { });
        Assert.Equal(TimeSpan.FromMilliseconds(1200), httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.NotNull(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(1200);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(TimeSpan.FromMilliseconds(1000), () => { });
        Assert.Equal(TimeSpan.FromMilliseconds(1000), httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.NotNull(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(TimeSpan.FromMilliseconds(1000));
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(-1);
        Assert.Equal(Timeout.InfiniteTimeSpan, httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout((TimeSpan?)null);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.Null(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(null, () => { });
        Assert.Null(httpRequestBuilder.TimeoutOptions?.Timeout);
        Assert.NotNull(httpRequestBuilder.TimeoutOptions?.OnTimeout);

        httpRequestBuilder.SetTimeout(new HttpTimeoutOptions());
        Assert.NotNull(httpRequestBuilder.TimeoutOptions);

        httpRequestBuilder.SetTimeout(_ => { });
        Assert.NotNull(httpRequestBuilder.TimeoutOptions);
    }
}