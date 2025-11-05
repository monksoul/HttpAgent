// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 远程请求断言上下文
/// </summary>
public sealed class HttpAssertionContext
{
    /// <summary>
    ///     响应内容字符串缓存
    /// </summary>
    internal string? _cachedContent;

    /// <summary>
    ///     <inheritdoc cref="HttpAssertionContext" />
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    internal HttpAssertionContext(HttpResponseMessage httpResponseMessage, long requestDuration,
        IServiceProvider serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ResponseMessage = httpResponseMessage;
        RequestDuration = requestDuration;
        ServiceProvider = serviceProvider;

        StatusCode = httpResponseMessage.StatusCode;
        IsSuccessStatusCode = httpResponseMessage.IsSuccessStatusCode;
    }

    /// <inheritdoc cref="HttpResponseMessage" />
    public HttpResponseMessage ResponseMessage { get; }

    /// <summary>
    ///     请求耗时（毫秒）
    /// </summary>
    public long RequestDuration { get; }

    /// <summary>
    ///     <inheritdoc cref="IServiceProvider" />
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     是否请求成功
    /// </summary>
    public bool IsSuccessStatusCode { get; }

    /// <summary>
    ///     读取响应内容字符串
    /// </summary>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public async Task<string?> ReadAsStringAsync(CancellationToken cancellationToken = default)
    {
        _cachedContent ??= await ResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return _cachedContent;
    }
}