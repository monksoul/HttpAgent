// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpRemoteUtilityTests
{
    [Fact]
    public void AllSslProtocols_ReturnOK()
    {
#pragma warning disable SYSLIB0039
#pragma warning disable CS0618 // 类型或成员已过时
        Assert.Equal(
            SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Ssl2 | SslProtocols.Ssl3 | SslProtocols.Tls12 |
            SslProtocols.Tls13 | SslProtocols.None,
#pragma warning restore CS0618 // 类型或成员已过时
#pragma warning restore SYSLIB0039
            HttpRemoteUtility.AllSslProtocols);
    }

    [Fact]
    public void IgnoreSslErrors_ReturnOK()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpRemoteUtility.IgnoreSslErrors
        };

        Assert.NotNull(handler.ServerCertificateCustomValidationCallback);
    }

    [Fact]
    public void IgnoreSocketSslErrors_ReturnOK()
    {
        var handler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = HttpRemoteUtility.IgnoreSocketSslErrors
            }
        };

        Assert.NotNull(handler.SslOptions.RemoteCertificateValidationCallback);
    }

    [Fact]
    public async Task IPAddressConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = (context, token) =>
                HttpRemoteUtility.IPAddressConnectCallback(AddressFamily.Unspecified, context, token)
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task IPv4ConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.IPv4ConnectCallback
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task IPv6ConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.IPv6ConnectCallback
        });

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await httpClient.GetAsync("https://www.baidu.com");
            response.EnsureSuccessStatusCode();
        });
    }

    [Fact]
    public async Task UnspecifiedConnectCallback_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = HttpRemoteUtility.UnspecifiedConnectCallback
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ConnectWithLocalIPv4_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = (context, token) =>
                HttpRemoteUtility.ConnectWithLocalIPv4(IPAddress.Parse("192.168.0.103"), context, token)
        });

        var response = await httpClient.GetAsync("https://www.baidu.com");
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ConnectWithLocalIPv6_ReturnOK()
    {
        using var httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectCallback = (context, token) =>
                HttpRemoteUtility.ConnectWithLocalIPv6(IPAddress.Parse("192.168.0.103"), context, token)
        });

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var response = await httpClient.GetAsync("https://www.baidu.com");
            response.EnsureSuccessStatusCode();
        });
    }

    [Fact]
    public void ResolveJsonSerializationContext_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            HttpRemoteUtility.ResolveJsonSerializationContext(null!, null, null));

    [Fact]
    public void ResolveJsonSerializationContext_WithDefault_ReturnOK()
    {
        Assert.Equal(typeof(JsonModel),
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), null, null).ResultType);

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), httpResponseMessage, null);
        Assert.NotNull(jsonSerializationContext.JsonSerializerOptions);
        Assert.False(jsonSerializationContext.JsonSerializerOptions.IncludeFields);
        Assert.Equal(typeof(JsonModel), jsonSerializationContext.ResultType);
        Assert.NotNull(jsonSerializationContext.GetResultValue);

        var jsonSerializationContext2 =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), null, null);
        Assert.NotNull(jsonSerializationContext2.JsonSerializerOptions);
        Assert.False(jsonSerializationContext2.JsonSerializerOptions.IncludeFields);
        Assert.Equal(typeof(JsonModel), jsonSerializationContext2.ResultType);
        Assert.NotNull(jsonSerializationContext2.GetResultValue);
    }

    [Fact]
    public void ResolveJsonSerializationContext_WithHttpClientOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonResponseWrapper = new JsonResponseWrapper(typeof(ApiResult<>), "Data");
        });
        using var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), httpResponseMessage, serviceProvider);
        Assert.NotNull(jsonSerializationContext.JsonSerializerOptions);
        Assert.True(jsonSerializationContext.JsonSerializerOptions.IncludeFields);
        Assert.Equal(typeof(JsonModel), jsonSerializationContext.ResultType);
        Assert.NotNull(jsonSerializationContext.GetResultValue);

        serviceProvider.Dispose();
    }

    [Fact]
    public void ResolveJsonSerializationContext_WithHttpClientOptions_UseJsonResponseWrapper_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonResponseWrapper = new JsonResponseWrapper(typeof(ApiResult<>), "Data");
        });
        using var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"success":true,"data":{"id":10,"name":"furion"}}""");
        var httpResponseMessage = new HttpResponseMessage();
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPER_KEY, "TRUE");
        httpResponseMessage.RequestMessage = httpRequestMessage;
        httpResponseMessage.Content = stringContent;

        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), httpResponseMessage, serviceProvider);
        Assert.NotNull(jsonSerializationContext.JsonSerializerOptions);
        Assert.True(jsonSerializationContext.JsonSerializerOptions.IncludeFields);
        Assert.Equal(typeof(ApiResult<JsonModel>), jsonSerializationContext.ResultType);
        Assert.NotNull(jsonSerializationContext.GetResultValue);

        serviceProvider.Dispose();
    }

    [Fact]
    public void ResolveJsonSerializationContext_WithHttpRemoteOptions_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });
        using var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"id":10, "name":"furion"}""");
        var httpResponseMessage = new HttpResponseMessage();
        httpResponseMessage.Content = stringContent;

        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(JsonModel), httpResponseMessage, serviceProvider);
        Assert.NotNull(jsonSerializationContext.JsonSerializerOptions);
        Assert.True(jsonSerializationContext.JsonSerializerOptions.IncludeFields);
        Assert.Equal(typeof(JsonModel), jsonSerializationContext.ResultType);
        Assert.NotNull(jsonSerializationContext.GetResultValue);

        serviceProvider.Dispose();
    }

    [Fact]
    public void ResolveHttpClientOptions_ReturnOK()
    {
        Assert.Null(HttpRemoteUtility.ResolveHttpClientOptions(null, null));
        Assert.Null(HttpRemoteUtility.ResolveHttpClientOptions(new HttpResponseMessage(), null));

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        Assert.Null(HttpRemoteUtility.ResolveHttpClientOptions(new HttpResponseMessage(), serviceProvider));

        var services2 = new ServiceCollection();
        services2.AddHttpClient(string.Empty);
        using var serviceProvider2 = services2.BuildServiceProvider();
        Assert.NotNull(HttpRemoteUtility.ResolveHttpClientOptions(new HttpResponseMessage(), serviceProvider2));

        var httpResponseMessage = new HttpResponseMessage();
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.HTTP_CLIENT_NAME, "Github");
        httpResponseMessage.RequestMessage = httpRequestMessage;

        Assert.NotNull(HttpRemoteUtility.ResolveHttpClientOptions(httpResponseMessage, serviceProvider2));
    }

    [Fact]
    public void ResolveJsonSerializerOptions_ReturnOK()
    {
        var result1 = HttpRemoteUtility.ResolveJsonSerializerOptions(null, null, out var clientOptions1);
        Assert.Same(HttpRemoteOptions.JsonSerializerOptionsDefault, result1);
        Assert.Null(clientOptions1);

        var services = new ServiceCollection();
        using var spEmpty = services.BuildServiceProvider();
        var result2 = HttpRemoteUtility.ResolveJsonSerializerOptions(spEmpty, null, out var clientOptions2);
        Assert.Same(HttpRemoteOptions.JsonSerializerOptionsDefault, result2);
        Assert.Null(clientOptions2);

        services = new ServiceCollection();
        services.AddHttpRemote();
        using var spGlobal = services.BuildServiceProvider();
        var result3 = HttpRemoteUtility.ResolveJsonSerializerOptions(spGlobal, null, out var clientOptions3);
        Assert.NotNull(result3);
        Assert.NotNull(clientOptions3);
        Assert.Null(clientOptions3.JsonSerializerOptions);

        services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options => options.JsonSerializerOptions.IncludeFields = true);
        using var spGlobalCustom = services.BuildServiceProvider();
        var result4 = HttpRemoteUtility.ResolveJsonSerializerOptions(spGlobalCustom, null, out var clientOptions4);
        Assert.True(result4.IncludeFields);
        Assert.NotNull(clientOptions4);
        Assert.Null(clientOptions4.JsonSerializerOptions);

        services = new ServiceCollection();
        services.AddHttpClient("test");
        using var spClientOnly = services.BuildServiceProvider();
        var result5 = HttpRemoteUtility.ResolveJsonSerializerOptions(spClientOnly, "test", out var clientOptions5);
        Assert.NotNull(clientOptions5);
        Assert.Null(clientOptions5.JsonSerializerOptions);
        Assert.NotNull(result5);
        Assert.True(result5.PropertyNameCaseInsensitive);
        Assert.Same(JsonNamingPolicy.CamelCase, result5.PropertyNamingPolicy);

        services = new ServiceCollection();
        services.AddHttpClient("configured").ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonSerializerOptions.WriteIndented = true;
        });
        using var spConfigured = services.BuildServiceProvider();
        var result6 =
            HttpRemoteUtility.ResolveJsonSerializerOptions(spConfigured, "configured", out var clientOptions6);
        Assert.NotNull(clientOptions6);
        Assert.NotNull(clientOptions6.JsonSerializerOptions);
        Assert.True(result6.IncludeFields);
        Assert.True(result6.WriteIndented);
        Assert.Same(clientOptions6.JsonSerializerOptions, result6);

        services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options => options.JsonSerializerOptions.IncludeFields = false);
        services.AddHttpClient("override").ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
        });
        using var spOverride = services.BuildServiceProvider();
        var result7 = HttpRemoteUtility.ResolveJsonSerializerOptions(spOverride, "override", out var clientOptions7);
        Assert.True(result7.IncludeFields);
        Assert.NotNull(clientOptions7);
        Assert.NotNull(clientOptions7.JsonSerializerOptions);
        Assert.Same(clientOptions7.JsonSerializerOptions, result7);
        var globalOpts = spOverride.GetRequiredService<IOptionsMonitor<HttpRemoteOptions>>().CurrentValue;
        Assert.False(globalOpts.JsonSerializerOptions.IncludeFields);

        services = new ServiceCollection();
        services.AddHttpRemote().ConfigureOptions(options => options.JsonSerializerOptions.IncludeFields = true);
        services.AddHttpClient("unconfigured");
        services.AddHttpClient("emptyConfig").ConfigureOptions(_ => { });
        using var spFallback = services.BuildServiceProvider();

        var result8 =
            HttpRemoteUtility.ResolveJsonSerializerOptions(spFallback, "unconfigured", out var clientOptions8);
        Assert.True(result8.IncludeFields);
        Assert.NotNull(clientOptions8);
        Assert.Null(clientOptions8.JsonSerializerOptions);

        var result9 = HttpRemoteUtility.ResolveJsonSerializerOptions(spFallback, "emptyConfig", out var clientOptions9);
        Assert.True(result9.IncludeFields);
        Assert.NotNull(clientOptions9);
        Assert.NotNull(clientOptions9.JsonSerializerOptions);
        Assert.True(clientOptions9.JsonSerializerOptions.IncludeFields);
        Assert.Same(clientOptions9.JsonSerializerOptions, result9);
    }

    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
    }

    public class JsonModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}