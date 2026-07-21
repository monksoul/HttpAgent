// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     请求事件管道处理器
/// </summary>
/// <param name="serviceProvider">
///     <see cref="IServiceProvider" />
/// </param>
/// <param name="logger">
///     <see cref="IHttpRemoteLogger" />
/// </param>
internal sealed class RequestEventPipelineHandler(IServiceProvider serviceProvider, IHttpRemoteLogger logger)
    : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 获取当前 HttpClient 实例的配置名称的配置选项
        var httpClientOptions = serviceProvider.GetService<IOptionsMonitor<HttpClientOptions>>()
            ?.Get(httpRequestBuilder.HttpClientName);

        // 获取全局的 IHttpRequestEventHandler 事件处理程序
        var globalEventHandler = httpClientOptions?.HttpRequestEventHandler;

        // 解析 IHttpRequestEventHandler 事件处理程序
        var requestEventHandler =
            (httpRequestBuilder.RequestEventHandlerType is not null
                ? serviceProvider.GetService(httpRequestBuilder.RequestEventHandlerType)
                : null) as IHttpRequestEventHandler;

        // 存入上下文
        context.Items[Constants.REQUEST_EVENT_HANDLER_KEY] = requestEventHandler;

        try
        {
            // 调用下一个处理器的委托
            return await next();
        }
        catch (Exception e)
        {
            // 输出请求异常日志
            logger.LogError(e, "An error occurred while sending HTTP request to {Url} using {Method}.",
                context.RequestMessage?.RequestUri?.ToString() ?? "unknown", context.RequestMessage?.Method);

            // 处理发送 HTTP 请求发生异常
            HandleRequestFailed(httpRequestBuilder, globalEventHandler, requestEventHandler, e,
                context.ResponseMessage);

            throw;
        }
        finally
        {
            // 处理收到 HTTP 响应之后
            HandlePostReceiveResponse(httpRequestBuilder, globalEventHandler, requestEventHandler,
                context.ResponseMessage);
        }
    }

    /// <summary>
    ///     处理发送 HTTP 请求发生异常
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="globalEventHandler"><see cref="HttpClientOptions" /> 配置 <see cref="IHttpRequestEventHandler" /></param>
    /// <param name="requestEventHandler">
    ///     <see cref="IHttpRequestEventHandler" />
    /// </param>
    /// <param name="e">
    ///     <see cref="Exception" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    internal static void HandleRequestFailed(HttpRequestBuilder httpRequestBuilder,
        IHttpRequestEventHandler? globalEventHandler, IHttpRequestEventHandler? requestEventHandler, Exception e,
        HttpResponseMessage? httpResponseMessage)
    {
        // 空检查
        if (globalEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(globalEventHandler.OnRequestFailed, e, httpResponseMessage);
        }

        // 空检查
        if (requestEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(requestEventHandler.OnRequestFailed, e, httpResponseMessage);
        }

        httpRequestBuilder.OnRequestFailed.TryInvoke(e, httpResponseMessage);
    }

    /// <summary>
    ///     处理收到 HTTP 响应之后
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="globalEventHandler"><see cref="HttpClientOptions" /> 配置 <see cref="IHttpRequestEventHandler" /></param>
    /// <param name="requestEventHandler">
    ///     <see cref="IHttpRequestEventHandler" />
    /// </param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    internal static void HandlePostReceiveResponse(HttpRequestBuilder httpRequestBuilder,
        IHttpRequestEventHandler? globalEventHandler, IHttpRequestEventHandler? requestEventHandler,
        HttpResponseMessage? httpResponseMessage)
    {
        // 空检查
        if (httpResponseMessage is null)
        {
            return;
        }

        // 空检查
        if (globalEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(globalEventHandler.OnPostReceiveResponse, httpResponseMessage);
        }

        // 空检查
        if (requestEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(requestEventHandler.OnPostReceiveResponse, httpResponseMessage);
        }

        httpRequestBuilder.OnPostReceiveResponse.TryInvoke(httpResponseMessage);
    }
}