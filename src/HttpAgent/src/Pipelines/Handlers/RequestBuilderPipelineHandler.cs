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
    IOptionsMonitor<HttpRemoteOptions> httpRemoteOptions) : IHttpRequestPipelineHandler
{
    /// <inheritdoc />
    public async Task<HttpResponseMessage?> HandleAsync(HttpRequestPipelineContext context,
        Func<Task<HttpResponseMessage?>> next)
    {
        // 获取当前 HttpRequestBuilder 实例
        var httpRequestBuilder = context.Builder;

        // 构建 HttpRequestMessage 实例
        var httpRequestMessage = httpRequestBuilder.Build(httpRemoteOptions.CurrentValue, httpContentProcessorFactory,
            context.HttpClient.BaseAddress ?? httpRemoteOptions.CurrentValue.FallbackBaseAddress);

        // 将 HttpCompletionOption 写入请求选项，供请求分析工具使用
        httpRequestMessage.Options.Set(
            new HttpRequestOptionsKey<HttpCompletionOption>(Constants.HTTP_COMPLETION_OPTION_KEY),
            context.CompletionOption);

        // 更新上下文
        context.RequestMessage = httpRequestMessage;

        // 执行请求断言委托操作
        await ExecuteAssertionsAsync(httpRequestBuilder, httpRequestMessage, serviceProvider);

        // 获取当前 HttpClient 实例的配置名称的配置选项
        var httpClientOptions =
            HttpRemoteUtility.ResolveHttpClientOptions(serviceProvider, httpRequestBuilder.HttpClientName);

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

    /// <summary>
    ///     执行请求断言委托操作
    /// </summary>
    /// <param name="httpRequestBuilder">
    ///     <see cref="HttpRequestBuilder" />
    /// </param>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />
    /// </param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    internal static async Task ExecuteAssertionsAsync(HttpRequestBuilder httpRequestBuilder,
        HttpRequestMessage httpRequestMessage, IServiceProvider serviceProvider)
    {
        // 检查是否配置了请求断言委托集合
        if (httpRequestBuilder.RequestAssertions is { Count: > 0 })
        {
            // 初始化 HttpAssertionContext 实例
            var requestAssertionContext = new HttpAssertionContext(null, httpRequestMessage, 0, serviceProvider);

            // 逐个调用请求断言委托
            foreach (var httpAssertion in httpRequestBuilder.RequestAssertions)
            {
                await httpAssertion(requestAssertionContext);
            }
        }
    }
}