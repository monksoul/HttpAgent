// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     提供 HTTP 远程请求实用方法
/// </summary>
public static class HttpRemoteUtility
{
    /// <summary>
    ///     获取所有支持的 SslProtocols
    /// </summary>
#pragma warning disable SYSLIB0039
#pragma warning disable CS0618 // 类型或成员已过时
    public static SslProtocols AllSslProtocols => SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Ssl2 |
                                                  SslProtocols.Ssl3 | SslProtocols.Tls12 | SslProtocols.Tls13 |
                                                  SslProtocols.None;
#pragma warning restore CS0618 // 类型或成员已过时
#pragma warning restore SYSLIB0039

    /// <summary>
    ///     忽略 SSL 证书验证
    /// </summary>
    public static Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> IgnoreSslErrors =>
        (_, _, _, _) => true;

    /// <summary>
    ///     忽略（Socket） SSL 证书验证
    /// </summary>
    public static RemoteCertificateValidationCallback IgnoreSocketSslErrors => (_, _, _, _) => true;

    /// <summary>
    ///     获取使用 IPv4 连接到服务器的回调
    /// </summary>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static ValueTask<Stream> IPv4ConnectCallback(SocketsHttpConnectionContext context,
        CancellationToken cancellationToken) =>
        IPAddressConnectCallback(AddressFamily.InterNetwork, context, cancellationToken);

    /// <summary>
    ///     获取使用 IPv6 连接到服务器的回调
    /// </summary>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static ValueTask<Stream> IPv6ConnectCallback(SocketsHttpConnectionContext context,
        CancellationToken cancellationToken) =>
        IPAddressConnectCallback(AddressFamily.InterNetworkV6, context, cancellationToken);

    /// <summary>
    ///     获取使用 IPv4 或 IPv6 连接到服务器的回调
    /// </summary>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static ValueTask<Stream> UnspecifiedConnectCallback(SocketsHttpConnectionContext context,
        CancellationToken cancellationToken) =>
        IPAddressConnectCallback(AddressFamily.Unspecified, context, cancellationToken);

    /// <summary>
    ///     根据 HTTP 响应消息和服务提供器，解析出 <see cref="HttpClient" /> 客户端对应的 JSON 响应反序列化时的上下文信息
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="Tuple{T1,T2,T3}" />
    /// </returns>
    public static (Type ResultType, JsonSerializerOptions JsonSerializerOptions, Func<object?, object?> GetResultValue)
        ResolveJsonSerializationContext(Type resultType, HttpResponseMessage? httpResponseMessage,
            IServiceProvider? serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resultType);

        // 获取 HttpClient 实例的配置名称
        if (httpResponseMessage?.RequestMessage?.Options.TryGetValue(
                new HttpRequestOptionsKey<string>(Constants.HTTP_CLIENT_NAME), out var httpClientName) != true)
        {
            httpClientName = string.Empty;
        }

        // 获取 HttpClientOptions 实例
        var httpClientOptions = serviceProvider?.GetService<IOptionsMonitor<HttpClientOptions>>()?.Get(httpClientName);

        // 获取 JsonSerializerOptions 配置
        // 优先级：指定名称的 HttpClientOptions -> HttpRemoteOptions -> 默认值
        var jsonSerializerOptions =
            (httpClientOptions?.IsDefault != false ? null : httpClientOptions.JsonSerializerOptions) ??
            serviceProvider?.GetService<IOptions<HttpRemoteOptions>>()?.Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault;

        // 检查是否禁用 JSON 响应反序列化包装器
        var disableJsonResponseWrapping = httpResponseMessage?.RequestMessage?.Options.TryGetValue(
            new HttpRequestOptionsKey<string>(Constants.DISABLE_JSON_RESPONSE_WRAPPING_KEY),
            out var disabledValue) == true && disabledValue == "TRUE";

        // 获取指定 JSON 响应反序列化包装器实例
        var jsonResponseWrapper = disableJsonResponseWrapping ? null : httpClientOptions?.JsonResponseWrapper;
        var jsonResponseWrapperType = jsonResponseWrapper?.GenericType;

        // 解析出最终返回的 JSON 响应结果类型
        var jsonResponseType = jsonResponseWrapperType is null
            ? resultType
            : jsonResponseWrapperType.MakeGenericType(resultType);

        return (jsonResponseType, jsonSerializerOptions,
            jsonResponseWrapper is null ? u => u : jsonResponseWrapper.GetResultValue);
    }

    /// <summary>
    ///     获取使用指定 IP 地址类型连接到服务器的回调
    /// </summary>
    /// <param name="addressFamily">
    ///     <see cref="AddressFamily" />
    /// </param>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    internal static async ValueTask<Stream> IPAddressConnectCallback(AddressFamily addressFamily,
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken)
    {
        // 参考文献：
        // - https://www.meziantou.net/forcing-httpclient-to-use-ipv4-or-ipv6-addresses.htm
        // - https://learn.microsoft.com/en-us/dotnet/core/runtime-config/#runtimeconfigjson

        // 使用 DNS 查找目标主机的 IP 地址：
        // - IPv4: AddressFamily.InterNetwork
        // - IPv6: AddressFamily.InterNetworkV6
        // - IPv4 或 IPv6: AddressFamily.Unspecified

        IPAddress[] addresses;

        // 当主机是一个 IP 地址，无需进一步解析
        if (IPAddress.TryParse(context.DnsEndPoint.Host, out var ipAddress))
        {
            addresses = [ipAddress];
        }
        else
        {
            // 注意：当主机没有 IP 地址时，此方法会抛出一个 SocketException 异常
            var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, addressFamily, cancellationToken);
            addresses = entry.AddressList;
        }

        // 打开与目标主机/端口的连接
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        // 关闭 Nagle 算法，因为这在大多数 HttpClient 场景中会降低性能。
        socket.NoDelay = true;

        try
        {
            await socket.ConnectAsync(addresses, context.DnsEndPoint.Port, cancellationToken);

            // 如果你想选择特定的 IP 地址来连接服务器
            // await socket.ConnectAsync(
            //    entry.AddressList[Random.Shared.Next(0, entry.AddressList.Length)],
            //    context.DnsEndPoint.Port, cancellationToken);

            // 返回 NetworkStream 给调用者
            return new NetworkStream(socket, true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }
}