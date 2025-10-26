// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpServerSentEventsBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(null);
        Assert.NotNull(builder);
        Assert.Null(builder.RequestUri);

        var builder2 = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.NotNull(builder2);
        Assert.NotNull(builder2.RequestUri);
        Assert.Equal(HttpMethod.Get, builder2.HttpMethod);
        Assert.Equal("http://localhost/", builder2.RequestUri.ToString());
        Assert.Equal(2000, builder2.DefaultRetryInterval);
        Assert.Equal(100, builder2.MaxRetries);
        Assert.Null(builder2.OnOpen);
        Assert.Null(builder2.OnMessage);
        Assert.Null(builder2.OnError);
        Assert.Null(builder2.ServerSentEventsEventHandlerType);
        Assert.Null(builder2._requestConfigure);

        var builder3 = new HttpServerSentEventsBuilder(HttpMethod.Post, new Uri("http://localhost"));
        Assert.NotNull(builder3);
        Assert.NotNull(builder3.RequestUri);
        Assert.Equal(HttpMethod.Post, builder3.HttpMethod);
    }

    [Fact]
    public void SetDefaultRetryInterval_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetDefaultRetryInterval(-1));
        Assert.Equal("Retry interval must be greater than 0. (Parameter 'retryInterval')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetDefaultRetryInterval(0));
        Assert.Equal("Retry interval must be greater than 0. (Parameter 'retryInterval')", exception2.Message);
    }

    [Fact]
    public void SetDefaultRetryInterval_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetDefaultRetryInterval(5000);
        Assert.Equal(5000, builder.DefaultRetryInterval);
    }

    [Fact]
    public void SetMaxRetries_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));

        var exception = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(-1));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.SetMaxRetries(0));
        Assert.Equal("Max retries must be greater than 0. (Parameter 'maxRetries')", exception2.Message);
    }

    [Fact]
    public void SetMaxRetries_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetMaxRetries(5);
        Assert.Equal(5, builder.MaxRetries);
    }

    [Fact]
    public void SetOnOpen_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnOpen(null!));
    }

    [Fact]
    public void SetOnOpen_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetOnOpen(() => { });
        Assert.NotNull(builder.OnOpen);
    }

    [Fact]
    public void SetOnMessage_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnMessage(null!));
    }

    [Fact]
    public void SetOnMessage_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetOnMessage(async (_, _) => await Task.CompletedTask);
        Assert.NotNull(builder.OnMessage);
    }

    [Fact]
    public void SetOnError_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.SetOnError(null!));
    }

    [Fact]
    public void SetOnError_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetOnError(_ => { });
        Assert.NotNull(builder.OnError);
    }

    [Fact]
    public void SetEventHandler_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));

        Assert.Throws<ArgumentNullException>(() => builder.SetEventHandler(null!));
        var exception = Assert.Throws<ArgumentException>(() =>
            builder.SetEventHandler(typeof(NotImplementServerSentEventsEventHandler)));
        Assert.Equal(
            $"`{typeof(NotImplementServerSentEventsEventHandler)}` type is not assignable from `{typeof(IHttpServerSentEventsEventHandler)}`. (Parameter 'serverSentEventsEventHandlerType')",
            exception.Message);
    }

    [Fact]
    public void SetEventHandler_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetEventHandler(typeof(CustomServerSentEventsEventHandler));

        Assert.Equal(typeof(CustomServerSentEventsEventHandler), builder.ServerSentEventsEventHandlerType);

        var builder2 = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder2.SetEventHandler<CustomServerSentEventsEventHandler>();

        Assert.Equal(typeof(CustomServerSentEventsEventHandler), builder2.ServerSentEventsEventHandlerType);
    }

    [Fact]
    public void WithRequest_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.WithRequest(null!));
    }

    [Fact]
    public void WithRequest_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Null(builder._requestConfigure);
        builder.WithRequest(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder._requestConfigure);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.Profiler();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(false);
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(_ => { });
        builder._requestConfigure?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new HttpServerSentEventsBuilder(new Uri("http://localhost")).Build(null!));

    [Fact]
    public void Build_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri("http://localhost")).Profiler();

        var httpRequestBuilder = httpServerSentEventsBuilder.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder);
        Assert.Equal(HttpMethod.Get, httpRequestBuilder.HttpMethod);
        Assert.NotNull(httpRequestBuilder.RequestUri);
        Assert.Equal("http://localhost/", httpRequestBuilder.RequestUri.ToString());
        Assert.True(httpRequestBuilder.EnsureSuccessStatusCodeEnabled);
        Assert.Null(httpRequestBuilder.RequestEventHandlerType);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);
        Assert.True(httpRequestBuilder.HttpClientPoolingEnabled);
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Single(httpRequestBuilder.Headers);
        Assert.Equal("Accept", httpRequestBuilder.Headers.Keys.First());
        Assert.Equal("text/event-stream", httpRequestBuilder.Headers["Accept"].First());
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        var httpRequestBuilder2 = httpServerSentEventsBuilder.SetEventHandler<CustomServerSentEventsEventHandler2>()
            .WithRequest(builder => builder.SetTimeout(100)).Build(httpRemoteOptions);

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }
}