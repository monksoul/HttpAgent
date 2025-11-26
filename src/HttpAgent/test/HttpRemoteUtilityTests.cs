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
        var serviceProvider = services.BuildServiceProvider();

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
    public void ResolveJsonSerializationContext_WithHttpClientOptions_WithEnableJsonResponseWrapping_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddHttpClient(string.Empty).ConfigureOptions(options =>
        {
            options.JsonSerializerOptions.IncludeFields = true;
            options.JsonResponseWrapper = new JsonResponseWrapper(typeof(ApiResult<>), "Data");
        });
        var serviceProvider = services.BuildServiceProvider();

        using var stringContent = new StringContent("""{"success":true,"data":{"id":10,"name":"furion"}}""");
        var httpResponseMessage = new HttpResponseMessage();
        var httpRequestMessage = new HttpRequestMessage();
        httpRequestMessage.Options.AddOrUpdate(Constants.ENABLE_JSON_RESPONSE_WRAPPING_KEY, "TRUE");
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
        var serviceProvider = services.BuildServiceProvider();

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