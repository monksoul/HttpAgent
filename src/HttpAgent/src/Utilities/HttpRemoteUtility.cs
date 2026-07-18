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
    ///     获取绑定到指定本地 IP 并使用 IPv4 连接到服务器的回调
    /// </summary>
    /// <remarks>适用于双网卡场景。</remarks>
    /// <param name="localIpAddress">要绑定的本地 IP 地址（即指定出口网卡）</param>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static ValueTask<Stream> ConnectWithLocalIPv4(IPAddress? localIpAddress,
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken) =>
        IPAddressConnectCallback(localIpAddress, AddressFamily.InterNetwork, context, cancellationToken);

    /// <summary>
    ///     获取绑定到指定本地 IP 并使用 IPv6 连接到服务器的回调
    /// </summary>
    /// <remarks>适用于双网卡场景。</remarks>
    /// <param name="localIpAddress">要绑定的本地 IP 地址（即指定出口网卡）</param>
    /// <param name="context">
    ///     <see cref="SocketsHttpConnectionContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="Stream" />
    /// </returns>
    public static ValueTask<Stream> ConnectWithLocalIPv6(IPAddress? localIpAddress,
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken) => IPAddressConnectCallback(localIpAddress, AddressFamily.InterNetworkV6,
        context, cancellationToken);

    /// <summary>
    ///     解析 <see cref="HttpClient" /> 客户端对应的 JSON 序列化上下文信息
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="JsonSerializationContext" />
    /// </returns>
    public static JsonSerializationContext ResolveJsonSerializationContext(Type resultType,
        HttpResponseMessage? httpResponseMessage, IServiceProvider? serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resultType);

        // 根据 HTTP 响应消息和服务提供器，解析出 HttpClient 客户端配置选项
        var httpClientOptions = ResolveHttpClientOptions(httpResponseMessage, serviceProvider);

        // 获取 JsonSerializerOptions 配置
        // 优先级：指定名称的 HttpClientOptions -> HttpRemoteOptions -> 默认值
        var jsonSerializerOptions =
            (httpClientOptions?.IsDefault != false ? null : httpClientOptions.JsonSerializerOptions) ??
            serviceProvider?.GetService<IOptions<HttpRemoteOptions>>()?.Value.JsonSerializerOptions ??
            HttpRemoteOptions.JsonSerializerOptionsDefault;

        // 检查是否启用 JSON 响应反序列化包装器并获取指定 JSON 响应反序列化包装器实例
        var jsonResponseWrapper = httpResponseMessage.ShouldUseJsonResponseWrapper(serviceProvider)
            ? httpClientOptions?.JsonResponseWrapper
            : null;
        var jsonResponseWrapperType = jsonResponseWrapper?.GenericType;

        // 解析出最终返回的 JSON 响应结果类型
        var jsonResponseType = jsonResponseWrapperType is null
            ? resultType
            : jsonResponseWrapperType.MakeGenericType(resultType);

        return new JsonSerializationContext(jsonResponseType, jsonSerializerOptions,
            jsonResponseWrapper is null ? (u, _) => u : jsonResponseWrapper.GetResultValue);
    }

    /// <summary>
    ///     获取使用指定 IP 地址类型连接到服务器的回调
    /// </summary>
    /// <remarks>自动选择本地出口。</remarks>
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
    public static ValueTask<Stream> IPAddressConnectCallback(AddressFamily addressFamily,
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken)
        => IPAddressConnectCallback(null, addressFamily, context, cancellationToken);

    /// <summary>
    ///     获取使用指定 IP 地址类型及可选本地 IP 绑定的连接回调
    /// </summary>
    /// <param name="localIpAddress">要绑定的本地 IP 地址（用于指定出口网卡），可为 null</param>
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
    public static async ValueTask<Stream> IPAddressConnectCallback(IPAddress? localIpAddress,
        AddressFamily addressFamily, SocketsHttpConnectionContext context, CancellationToken cancellationToken)
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

        // 若指定了本地 IP，则以其地址族为准；否则使用传入的 addressFamily（若为 Unspecified，默认回退到 IPv4）
        var socketAddressFamily = localIpAddress?.AddressFamily ??
                                  (addressFamily == AddressFamily.Unspecified
                                      ? AddressFamily.InterNetwork
                                      : addressFamily);

        // 打开与目标主机/端口的连接
        var socket = new Socket(socketAddressFamily, SocketType.Stream, ProtocolType.Tcp);

        // 关闭 Nagle 算法，因为这在大多数 HttpClient 场景中会降低性能
        socket.NoDelay = true;

        try
        {
            // 如果指定了本地 IP，则绑定到该地址（强制使用指定网卡出口）
            if (localIpAddress != null)
            {
                socket.Bind(new IPEndPoint(localIpAddress, 0));
            }

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

    /// <summary>
    ///     获取本机第一个活跃的非回环 IPv4 地址
    /// </summary>
    /// <remarks>如果遍历所有本机网络接口后仍无可用地址，则返回安全回退值 <c>127.0.0.1</c>。</remarks>
    /// <param name="defaultAddress">缺省 IPv4 地址，默认值为：<c>127.0.0.1</c></param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string GetLocalIPv4(string defaultAddress = "127.0.0.1")
    {
        try
        {
            // 遍历系统中所有网络接口（以太网、Wi-Fi、虚拟网卡等）
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 排除未启用的接口
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                // 排除回环接口和隧道接口
                if (networkInterface.NetworkInterfaceType is NetworkInterfaceType.Loopback
                    or NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                // 遍历该接口上的所有单播 IP 地址
                foreach (var ipAddress in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    // 检查是否是 IPv4 地址
                    if (ipAddress.Address.AddressFamily is AddressFamily.InterNetwork)
                    {
                        return ipAddress.Address.ToString();
                    }
                }
            }
        }
        catch (NetworkInformationException)
        {
        }

        return defaultAddress;
    }

    /// <summary>
    ///     获取本机第一个活跃的非回环网卡的 MAC 地址
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public static string? GetLocalMacAddress()
    {
        try
        {
            // 遍历系统中所有网络接口（以太网、Wi-Fi、虚拟网卡等）
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 排除未启用的接口
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                // 排除回环接口和隧道接口
                if (networkInterface.NetworkInterfaceType is NetworkInterfaceType.Loopback
                    or NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                // 获取物理地址
                var physicalAddress = networkInterface.GetPhysicalAddress();
                var bytes = physicalAddress.GetAddressBytes();

                // 空检查
                if (bytes.Length == 0)
                {
                    continue;
                }

                // 格式化为标准的 MAC 地址字符串
                return string.Join("-", bytes.Select(b => b.ToString("X2")));
            }
        }
        catch (NetworkInformationException)
        {
        }

        return null;
    }

    /// <summary>
    ///     根据 HTTP 响应消息和服务提供器，解析出 <see cref="HttpClient" /> 客户端配置选项
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="HttpClientOptions" />
    /// </returns>
    internal static HttpClientOptions? ResolveHttpClientOptions(HttpResponseMessage? httpResponseMessage,
        IServiceProvider? serviceProvider) =>
        serviceProvider?.GetService<IOptionsMonitor<HttpClientOptions>>()
            ?.Get(httpResponseMessage.ResolveHttpClientName());
}