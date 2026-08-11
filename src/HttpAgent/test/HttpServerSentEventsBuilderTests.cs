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
        Assert.True(builder2.AutoCorrectMethod);
        Assert.Null(builder2._configureRequest);
        Assert.Null(builder2.Configure);
        Assert.Same(builder2, builder2.This);

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
    public void With_Invalid_Parameters()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Throws<ArgumentNullException>(() => builder.With(null!));
    }

    [Fact]
    public void With_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.Null(builder._configureRequest);
        builder.With(requestBuilder => requestBuilder.WithHeader("framework", "Furion"));
        Assert.NotNull(builder._configureRequest);
        Assert.NotNull(builder.Configure);
    }

    [Fact]
    public void Profiler_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.Profiler();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(false);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.ProfilerEnabled);

        builder.Profiler(_ => { });
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.ProfilerEnabled);
    }

    [Fact]
    public void DisableCache_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.DisableCache();

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.True(httpRequestBuilder.DisableCacheEnabled);

        builder.DisableCache(false);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.False(httpRequestBuilder.DisableCacheEnabled);
    }

    [Fact]
    public void AddBearerAuthentication_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.AddBearerAuthentication("token");

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.AuthenticationHeader);
        Assert.Equal("Bearer", httpRequestBuilder.AuthenticationHeader.Scheme);

        builder.AddBearerAuthentication("x-header", "token");
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.StartsWith("Bearer", httpRequestBuilder.Headers["x-header"].First());
    }

    [Fact]
    public void SetJsonContent_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetJsonContent(new { });

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);

        builder.SetJsonContentWithoutValidation("[]");
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);
    }

    [Fact]
    public void SetContent_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        builder.SetContent(new { });

        var httpRequestBuilder = new HttpRequestBuilder(HttpMethod.Get, null);
        builder._configureRequest?.Invoke(httpRequestBuilder);
        Assert.NotNull(httpRequestBuilder.RawContent);
    }

    [Fact]
    public void SetAutoCorrectMethod_ReturnOK()
    {
        var builder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        Assert.True(builder.AutoCorrectMethod);

        builder.SetAutoCorrectMethod(false);
        Assert.False(builder.AutoCorrectMethod);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new HttpServerSentEventsBuilder(new Uri("http://localhost")).Build(null!));

    [Fact]
    public void SetMcpContent_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri("http://localhost"));

        httpServerSentEventsBuilder.SetMcpContent("mcptools", new McpMessageData("list"));
        var httpRequestBuilder = httpServerSentEventsBuilder.Build(httpRemoteOptions);

        Assert.NotNull(httpRequestBuilder.Headers);
        Assert.Equal(4, httpRequestBuilder.Headers.Count);
        Assert.Equal("mcptools", httpRequestBuilder.Headers["Mcp-Name"].FirstOrDefault());
        Assert.Equal("2026-07-28", httpRequestBuilder.Headers["MCP-Protocol-Version"].FirstOrDefault());
        Assert.Equal("list", httpRequestBuilder.Headers["Mcp-Method"].FirstOrDefault());
        Assert.Equal("application/json, text/event-stream", httpRequestBuilder.Headers["Accept"].FirstOrDefault());
        Assert.NotNull(httpRequestBuilder.RawContent);
        Assert.Equal("{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"list\",\"params\":null}",
            httpRequestBuilder.RawContent.ToJsonString());
        Assert.Equal("application/json", httpRequestBuilder.ContentType);

        var httpServerSentEventsBuilder2 = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        httpServerSentEventsBuilder2.SetMcpContent("mcptools", new McpMessageData("list", new { data = "furion" }));
        var httpRequestBuilder2 = httpServerSentEventsBuilder2.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder2.RawContent);
        Assert.Equal("{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"list\",\"params\":{\"data\":\"furion\"}}",
            httpRequestBuilder2.RawContent.ToJsonString());

        var httpServerSentEventsBuilder3 = new HttpServerSentEventsBuilder(new Uri("http://localhost"));
        httpServerSentEventsBuilder3.SetMcpContent("mcptools", "list", new { data = "furion" });
        var httpRequestBuilder3 = httpServerSentEventsBuilder3.Build(httpRemoteOptions);
        Assert.NotNull(httpRequestBuilder3.RawContent);
        Assert.Equal("{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"list\",\"params\":{\"data\":\"furion\"}}",
            httpRequestBuilder3.RawContent.ToJsonString());
    }

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
            .With(builder => builder.SetTimeout(100)).Build(httpRemoteOptions);

        Assert.Equal(TimeSpan.FromMilliseconds(100), httpRequestBuilder2.TimeoutOptions?.Timeout);
        Assert.NotNull(httpRequestBuilder2.RequestEventHandlerType);
    }

    [Fact]
    public void Build_AutoCorrectMethod_ReturnOK()
    {
        var httpRemoteOptions = new HttpRemoteOptions();
        var httpServerSentEventsBuilder = new HttpServerSentEventsBuilder(new Uri("http://localhost")).Profiler();
        httpServerSentEventsBuilder.SetJsonContent("{}");
        Assert.Equal(HttpMethod.Get, httpServerSentEventsBuilder.HttpMethod);

        var httpRequestBuilder = httpServerSentEventsBuilder.Build(httpRemoteOptions);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder.HttpMethod);

        var httpServerSentEventsBuilder2 =
            new HttpServerSentEventsBuilder(HttpMethod.Head, new Uri("http://localhost")).Profiler();
        httpServerSentEventsBuilder2.SetJsonContent("{}");
        Assert.Equal(HttpMethod.Head, httpServerSentEventsBuilder2.HttpMethod);

        var httpRequestBuilder2 = httpServerSentEventsBuilder2.Build(httpRemoteOptions);
        Assert.Equal(HttpMethod.Post, httpRequestBuilder2.HttpMethod);
    }
}