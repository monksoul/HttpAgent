// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 请求管道上下文
/// </summary>
public sealed class HttpRequestPipelineContext
{
    /// <summary>
    ///     <inheritdoc cref="HttpRequestPipelineContext" />
    /// </summary>
    /// <param name="originalBuilder">原始的 <see cref="HttpRequestBuilder" /></param>
    /// <param name="httpClient">
    ///     <see cref="HttpClient" />
    /// </param>
    /// <param name="completionOption">
    ///     <see cref="HttpCompletionOption" />
    /// </param>
    /// <param name="sendAsync">发送委托</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    internal HttpRequestPipelineContext(HttpRequestBuilder originalBuilder, HttpClient httpClient,
        HttpCompletionOption completionOption,
        Func<HttpClient, HttpRequestMessage, HttpCompletionOption, CancellationToken, Task<HttpResponseMessage>>
            sendAsync, CancellationToken cancellationToken)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(originalBuilder);
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(sendAsync);

        OriginalBuilder = originalBuilder;
        Builder = originalBuilder;
        HttpClient = httpClient;
        CompletionOption = completionOption;
        SendAsync = sendAsync;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    ///     原始的 <see cref="HttpRequestBuilder" />
    /// </summary>
    public HttpRequestBuilder OriginalBuilder { get; }

    /// <summary>
    ///     当前使用的 <see cref="HttpRequestBuilder" />
    /// </summary>
    public HttpRequestBuilder Builder { get; set; }

    /// <summary>
    ///     <see cref="HttpClient" />
    /// </summary>
    public HttpClient HttpClient { get; }

    /// <summary>
    ///     <see cref="HttpCompletionOption" />
    /// </summary>
    public HttpCompletionOption CompletionOption { get; }

    /// <summary>
    ///     当前有效的 <see cref="CancellationToken" />
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    ///     发送委托
    /// </summary>
    public Func<HttpClient, HttpRequestMessage, HttpCompletionOption, CancellationToken, Task<HttpResponseMessage>>
        SendAsync { get; }

    /// <summary>
    ///     最近一次构建的 <see cref="HttpRequestMessage" />
    /// </summary>
    public HttpRequestMessage? RequestMessage { get; set; }

    /// <summary>
    ///     最近一次收到的 <see cref="HttpResponseMessage" />
    /// </summary>
    public HttpResponseMessage? ResponseMessage { get; set; }

    /// <summary>
    ///     请求耗时（毫秒）
    /// </summary>
    public long RequestDuration { get; set; }

    /// <summary>
    ///     共享数据字典
    /// </summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
}