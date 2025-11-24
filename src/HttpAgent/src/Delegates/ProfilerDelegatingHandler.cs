// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求分析工具处理委托
/// </summary>
/// <remarks>参考文献：https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#outgoing-request-middleware</remarks>
/// <param name="logger">
///     <see cref="IHttpRemoteLogger" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="IOptions{TOptions}" />
/// </param>
public sealed class ProfilerDelegatingHandler(IHttpRemoteLogger logger, IOptions<HttpRemoteOptions> httpRemoteOptions)
    : DelegatingHandler
{
    /// <summary>
    ///     是否启用请求分析工具
    /// </summary>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsEnabled(HttpRequestMessage httpRequestMessage) =>
        !(httpRequestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>(Constants.DISABLED_PROFILER_KEY),
            out var value) && value == "TRUE");

    /// <inheritdoc />
    protected override HttpResponseMessage Send(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        // 检查是否启用请求分析工具
        if (!IsEnabled(httpRequestMessage))
        {
            return base.Send(httpRequestMessage, cancellationToken);
        }

        // 记录请求信息
        LogRequestAsync(logger, httpRemoteOptions.Value, httpRequestMessage, null, cancellationToken)
            .GetAwaiter().GetResult();

        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        // 发送 HTTP 远程请求
        var httpResponseMessage = base.Send(httpRequestMessage, cancellationToken);

        // 获取请求耗时
        var requestDuration = stopwatch.ElapsedMilliseconds;

        // 停止计时
        stopwatch.Stop();

        // 记录响应信息
        LogResponseAsync(logger, httpRemoteOptions.Value, httpResponseMessage, requestDuration, null, cancellationToken)
            .GetAwaiter().GetResult();

        // 打印 CookieContainer 内容
        LogCookieContainer(logger, httpRemoteOptions.Value, httpRequestMessage, ExtractCookieContainer());

        return httpResponseMessage;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        // 检查是否启用请求分析工具
        if (!IsEnabled(httpRequestMessage))
        {
            return await base.SendAsync(httpRequestMessage, cancellationToken);
        }

        // 记录请求信息
        await LogRequestAsync(logger, httpRemoteOptions.Value, httpRequestMessage, null, cancellationToken);

        // 初始化 Stopwatch 实例并开启计时操作
        var stopwatch = Stopwatch.StartNew();

        // 发送 HTTP 远程请求
        var httpResponseMessage = await base.SendAsync(httpRequestMessage, cancellationToken);

        // 获取请求耗时
        var requestDuration = stopwatch.ElapsedMilliseconds;

        // 停止计时
        stopwatch.Stop();

        // 记录响应信息
        await LogResponseAsync(logger, httpRemoteOptions.Value, httpResponseMessage, requestDuration,
            null, cancellationToken);

        // 打印 CookieContainer 内容
        LogCookieContainer(logger, httpRemoteOptions.Value, httpRequestMessage, ExtractCookieContainer());

        return httpResponseMessage;
    }

    /// <summary>
    ///     记录请求信息
    /// </summary>
    /// <param name="logger">
    ///     <see cref="IHttpRemoteLogger" />
    /// </param>
    /// <param name="remoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="request">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="httpRemoteAnalyzer">
    ///     <see cref="HttpRemoteAnalyzer" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal static async Task LogRequestAsync(IHttpRemoteLogger logger, HttpRemoteOptions remoteOptions,
        HttpRequestMessage request, HttpRemoteAnalyzer? httpRemoteAnalyzer = null,
        CancellationToken cancellationToken = default)
    {
        Log(logger, remoteOptions, request.ProfilerHeaders(), httpRemoteAnalyzer);
        Log(logger, remoteOptions, await request.Content.ProfilerAsync(cancellationToken: cancellationToken),
            httpRemoteAnalyzer);
    }

    /// <summary>
    ///     记录响应信息
    /// </summary>
    /// <param name="logger">
    ///     <see cref="IHttpRemoteLogger" />
    /// </param>
    /// <param name="remoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    /// <param name="httpRemoteAnalyzer">
    ///     <see cref="HttpRemoteAnalyzer" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    internal static async Task LogResponseAsync(IHttpRemoteLogger logger, HttpRemoteOptions remoteOptions,
        HttpResponseMessage httpResponseMessage, long requestDuration, HttpRemoteAnalyzer? httpRemoteAnalyzer = null,
        CancellationToken cancellationToken = default)
    {
        Log(logger, remoteOptions, httpResponseMessage.ProfilerGeneralAndHeaders(generalCustomKeyValues:
                [new KeyValuePair<string, IEnumerable<string>>("Request Duration (ms)", [$"{requestDuration:N2}"])]),
            httpRemoteAnalyzer);
        Log(logger, remoteOptions, await httpResponseMessage.Content.ProfilerAsync("Response Body", cancellationToken),
            httpRemoteAnalyzer);
    }

    /// <summary>
    ///     打印 <see cref="CookieContainer" /> 内容
    /// </summary>
    /// <param name="logger">
    ///     <see cref="IHttpRemoteLogger" />
    /// </param>
    /// <param name="remoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="request">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="cookieContainer">
    ///     <see cref="CookieContainer" />
    /// </param>
    /// <param name="httpRemoteAnalyzer">
    ///     <see cref="HttpRemoteAnalyzer" />
    /// </param>
    internal static void LogCookieContainer(IHttpRemoteLogger logger, HttpRemoteOptions remoteOptions,
        HttpRequestMessage request, CookieContainer? cookieContainer, HttpRemoteAnalyzer? httpRemoteAnalyzer = null)
    {
        // 空检查
        if (request.RequestUri is null || cookieContainer is null)
        {
            return;
        }

        // 获取 Cookie 集合
        var cookies = cookieContainer.GetCookies(request.RequestUri);

        // 空检查
        if (cookies is { Count: 0 })
        {
            return;
        }

        // 打印日志
        Log(logger, remoteOptions, StringUtility.FormatKeyValuesSummary(
            cookies.ToDictionary(u => u.Name, u => Enumerable.Empty<string>().Concat([u.Value])),
            "Cookie Container"), httpRemoteAnalyzer);
    }

    /// <summary>
    ///     打印日志
    /// </summary>
    /// <param name="logger">
    ///     <see cref="IHttpRemoteLogger" />
    /// </param>
    /// <param name="remoteOptions">
    ///     <see cref="HttpRemoteOptions" />
    /// </param>
    /// <param name="message">日志消息</param>
    /// <param name="httpRemoteAnalyzer">
    ///     <see cref="HttpRemoteAnalyzer" />
    /// </param>
    internal static void Log(IHttpRemoteLogger logger, HttpRemoteOptions remoteOptions, string? message,
        HttpRemoteAnalyzer? httpRemoteAnalyzer = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(logger);

        // 空检查
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        // 追加分析数据
        httpRemoteAnalyzer?.AppendData(message);

        // 记录日志
        logger.Log(remoteOptions.ProfilerLogLevel, null, message);
    }

    /// <summary>
    ///     提取 <see cref="CookieContainer" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="CookieContainer" />
    /// </returns>
    internal CookieContainer? ExtractCookieContainer()
    {
        // 检查是否是 WebAssembly 应用，如果是则跳过
        if (OperatingSystem.IsBrowser())
        {
            return null;
        }

        return InnerHandler switch
        {
            LoggingHttpMessageHandler loggingHttpMessageHandler => loggingHttpMessageHandler.InnerHandler switch
            {
                SocketsHttpHandler socketsHttpHandler => socketsHttpHandler.CookieContainer,
                HttpClientHandler httpClientHandler => httpClientHandler.CookieContainer,
                _ => null
            },
            LoggingScopeHttpMessageHandler loggingScopeHttpMessageHandler => loggingScopeHttpMessageHandler.InnerHandler
                switch
                {
                    SocketsHttpHandler socketsHttpHandler => socketsHttpHandler.CookieContainer,
                    HttpClientHandler httpClientHandler => httpClientHandler.CookieContainer,
                    _ => null
                },
            SocketsHttpHandler socketsHttpHandler => socketsHttpHandler.CookieContainer,
            HttpClientHandler httpClientHandler => httpClientHandler.CookieContainer,
            _ => null
        };
    }
}