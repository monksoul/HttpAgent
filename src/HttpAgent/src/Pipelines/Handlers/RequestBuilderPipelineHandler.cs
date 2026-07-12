// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     构建 <see cref="HttpRequestMessage" /> 管道处理器
/// </summary>
/// <param name="httpContentProcessorFactory">
///     <see cref="IHttpContentProcessorFactory" />
/// </param>
/// <param name="httpRemoteOptions">
///     <see cref="HttpRemoteOptions" />
/// </param>
internal sealed class RequestBuilderPipelineHandler(
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

        // 解析 IHttpRequestEventHandler 事件处理程序
        var requestEventHandler = context.Items.TryGetValue("RequestEventHandler", out var eventHandler)
            ? eventHandler as IHttpRequestEventHandler
            : null;

        // 处理发送 HTTP 请求之前
        HandlePreSendRequest(httpRequestBuilder, requestEventHandler, httpRequestMessage);

        // 调用下一个处理器的委托
        return await next();
    }

    /// <summary>
    ///     处理发送 HTTP 请求之前
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="requestEventHandler">
    ///     <see cref="IHttpRequestEventHandler" />
    /// </param>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    internal static void HandlePreSendRequest(HttpRequestBuilder httpRequestBuilder,
        IHttpRequestEventHandler? requestEventHandler, HttpRequestMessage httpRequestMessage)
    {
        // 空检查
        if (requestEventHandler is not null)
        {
            DelegateExtensions.TryInvoke(requestEventHandler.OnPreSendRequest, httpRequestMessage);
        }

        httpRequestBuilder.OnPreSendRequest.TryInvoke(httpRequestMessage);
    }
}