// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpLongPollingBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpLongPollingBuilder(null!, null));

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, null!);
        Assert.NotNull(builder);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.NotNull(builder2);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.Equal(TimeSpan.FromSeconds(2), builder2.RetryInterval);
        Assert.Null(builder2.Timeout);
        Assert.Equal(100, builder2.MaxRetries);
        Assert.Null(builder2.OnDataReceived);
        Assert.Null(builder2.OnError);
        Assert.Null(builder2.OnEndOfStream);
        Assert.Null(builder2.LongPollingEventHandlerType);
        Assert.Null(builder2.RequestConfigure);
    }

    [Fact]
    public void SetRetryInterval_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetRetryInterval(TimeSpan.Zero));
        Assert.Equal("Retry interval must be greater than 0. (Parameter 'retryInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetRetryInterval(TimeSpan.FromSeconds(-1)));
        Assert.Equal("Retry interval must be greater than 0. (Parameter 'retryInterval')", exception2.Message);
    }

    [Fact]
    public void SetRetryInterval_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetRetryInterval(TimeSpan.FromMilliseconds(6000));
        Assert.Equal(TimeSpan.FromMilliseconds(6000), builder.RetryInterval);
    }

    [Fact]
    public void SetMaxRetries_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(-1));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(0));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception2.Message);
    }

    [Fact]
    public void SetMaxRetries_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetMaxRetries(5);
        Assert.Equal(5, builder.MaxRetries);
    }

    [Fact]
    public void SetTimeout_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            builder.SetTimeout(-1);
        });

        Assert.Equal("Timeout value must be non-negative. (Parameter 'timeoutMilliseconds')", exception.Message);
    }

    [Fact]
    public void SetTimeout_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetTimeout(TimeSpan.MaxValue);
        Assert.Equal(TimeSpan.MaxValue, builder.Timeout);

        builder.SetTimeout(1000);
        Assert.Equal(TimeSpan.FromMilliseconds(1000), builder.Timeout);

        builder.SetTimeout(0);
        Assert.Equal(TimeSpan.Zero, builder.Timeout);
    }

    [Fact]
    public void SetOnDataReceived_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnDataReceived(null!));
    }

    [Fact]
    public void SetOnDataReceived_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetOnDataReceived(async (_, _) => await Task.CompletedTask);
        Assert.NotNull(builder.OnDataReceived);
    }

    [Fact]
    public void SetOnError_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnError(null!));
    }

    [Fact]
    public void SetOnError_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetOnError(async (_, _) => await Task.CompletedTask);
        Assert.NotNull(builder.OnError);
    }

    [Fact]
    public void SetOnEndOfStream_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnEndOfStream(null!));
    }

    [Fact]
    public void SetOnEndOfStream_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetOnEndOfStream(async (_, _) => await Task.CompletedTask);
        Assert.NotNull(builder.OnEndOfStream);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<ArgumentException>(() =>
            builder.SetEventHandler(typeof(NotLongPollingEventHandler)));
        Assert.Equal(
            $"`{typeof(NotLongPollingEventHandler)}` type is not assignable from `{typeof(IHttpLongPollingEventHandler)}`. (Parameter 'longPollingEventHandlerType')",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.SetEventHandler(typeof(CustomLongPollingEventHandler));

        Assert.Equal(typeof(CustomLongPollingEventHandler), builder.LongPollingEventHandlerType);

        var builder2 = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder2.SetEventHandler<CustomLongPollingEventHandler>();

        Assert.Equal(typeof(CustomLongPollingEventHandler), builder2.LongPollingEventHandlerType);
    }

    [Fact]
    public void WithRequest_Invalid_Parameters()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.WithRequest(null!));
    }

    [Fact]
    public void WithRequest_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        Assert.Null(builder.RequestConfigure);
        builder.WithRequest(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder.RequestConfigure);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var builder = new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost"));
        builder.Profiler();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder.RequestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(false);
        builder.RequestConfigure?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(_ => { });
        builder.RequestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost")).Build(null!));

    [Fact]
    public void Build_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var httpLongPollingBuilder =
            new HttpLongPollingBuilder(HttpMethod.Get, new Uri("http://localhost")).SetTimeout(
                TimeSpan.FromMilliseconds(100)).Profiler();

        var httpRequestBuilder = httpLongPollingBuilder.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.False(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);
        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder.Timeout);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        var httpRequestBuilder2 = httpLongPollingBuilder.SetEventHandler<CustomLongPollingEventHandler2>()
            .WithRequest(builder => builder.SetTimeout(100))
            .Build(httpRemoteOptions);

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }
}