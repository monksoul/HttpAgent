// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     构建 <see cref="HttpRequestMessage" /> 管道处理器
/// </summary>
/// <param name="serviceProvider">
///     <see cref="IServiceProvider" />
/// </param>
/// <param name="httpContentProcessorFactory">
///     <see cref="IHttpContentProcessorFactory" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="HttpRemoteOptions" />
/// </param>
internal sealed class RequestBuilderPipelineHandler(
    IServiceProvider serviceProvider,
    IHttpContentProcessorFactory httpContentProcessorFactory,
    IOptions<HttpRemoteOptions> httpRemoteOptions) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 构建 HttpRequestMessage 实例
        var httpRequestMessage = httpRequestBuilder.Build(httpRemoteOptions.Value, httpContentProcessorFactory,
            context.HttpClient.BaseAddress ?? httpRemoteOptions.Value.FallbackBaseAddress);

        // 更新上下文
        context.RequestMessage = httpRequestMessage;

        // 获取当前 HttpClient 实例的配置名称的配置选项
        var httpClientOptions = serviceProvider.GetService<IOptionsMonitor<HttpClientOptions>>()
            ?.Get(httpRequestBuilder.HttpClientName);

        // 获取全局的 IHttpRequestEventHandler 事件处理程序
        var globalEventHandler = httpClientOptions?.HttpRequestEventHandler;

        // 解析 IHttpRequestEventHandler 事件处理程序
        var requestEventHandler = context.Items.TryGetValue(Constants.REQUEST_EVENT_HANDLER_KEY, out var eventHandler)
            ? eventHandler as IHttpRequestEventHandler
            : null;

        // 处理发送 HTTP 请求之前
        HandlePreSendRequest(httpRequestBuilder, globalEventHandler, requestEventHandler, httpRequestMessage);

        // 调用下一个处理器的委托
        return await next();
    }

    /// <summary>
    ///     处理发送 HTTP 请求之前
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="globalEventHandler"><see cref="HttpClientOptions" /> 配置 <see cref="IHttpRequestEventHandler" /></param>
    /// <param name="requestEventHandler">
    ///     <see cref="IHttpRequestEventHandler" />
    /// </param>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal static void HandlePreSendRequest(HttpRequestBuilder httpRequestBuilder,
        IHttpRequestEventHandler? globalEventHandler, IHttpRequestEventHandler? requestEventHandler,
        HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (globalEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(globalEventHandler.OnPreSendRequest, httpRequestMessage);
        }

        // 空检查
        if (requestEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(requestEventHandler.OnPreSendRequest, httpRequestMessage);
        }

        httpRequestBuilder.OnPreSendRequest.TryInvoke(httpRequestMessage);
    }
}