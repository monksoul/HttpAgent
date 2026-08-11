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
    ///     请求内容字符串缓存
    /// </summary>
    internal string? _cachedRequestContent;

    /// <summary>
    ///     响应内容字符串缓存
    /// </summary>
    internal string? _cachedResponseContent;

    /// <summary>
    ///     <inheritdoc cref="HttpAssertionContext" />
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />，可为 <c>null</c>（用于请求断言阶段）
    /// </param>
    /// <param name="httpRequestMessage">
    ///     <see cref="HttpRequestMessage" />，可选，用于断言请求内容
    /// </param>
    /// <param name="requestDuration">请求耗时（毫秒）</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    internal HttpAssertionContext(HttpResponseMessage? httpResponseMessage,
        HttpRequestMessage? httpRequestMessage, long requestDuration, IServiceProvider serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceProvider);

        ResponseMessage = httpResponseMessage;
        RequestMessage = httpRequestMessage;
        RequestDuration = requestDuration;
        ServiceProvider = serviceProvider;

        StatusCode = httpResponseMessage?.StatusCode ?? default;
        IsSuccessStatusCode = httpResponseMessage?.IsSuccessStatusCode ?? false;
    }

    /// <inheritdoc cref="HttpResponseMessage" />
    public HttpResponseMessage? ResponseMessage { get; }

    /// <inheritdoc cref="HttpRequestMessage" />
    public HttpRequestMessage? RequestMessage { get; }

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
    /// <remarks>支持多次读取。</remarks>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<string?> ReadResponseAsStringAsync(CancellationToken cancellationToken = default)
    {
        // 空检查
        if (ResponseMessage is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ResponseMessage)} is null, cannot read response content.");
        }

        // 空检查
        if (_cachedResponseContent is not null)
        {
            return _cachedResponseContent;
        }

        // 启用缓冲，可重复读取
#if NET8_0
        await ResponseMessage.Content.LoadIntoBufferAsync();
#else
        await ResponseMessage.Content.LoadIntoBufferAsync(cancellationToken);
#endif

        _cachedResponseContent = await ResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        return _cachedResponseContent;
    }

    /// <summary>
    ///     读取请求内容字符串
    /// </summary>
    /// <remarks>支持多次读取。</remarks>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public async Task<string?> ReadRequestAsStringAsync(CancellationToken cancellationToken = default)
    {
        // 空检查
        if (RequestMessage?.Content is null)
        {
            return null;
        }

        // 空检查
        if (_cachedRequestContent is not null)
        {
            return _cachedRequestContent;
        }

        // 启用缓冲，可重复读取
        try
        {
#if NET8_0
            await RequestMessage.Content.LoadIntoBufferAsync();
#else
            await RequestMessage.Content.LoadIntoBufferAsync(cancellationToken);
#endif
        }
        catch
        {
            // ignored
        }

        // 读取流内容
        var stream = await RequestMessage.Content.ReadAsStreamAsync(cancellationToken);

        // 检查流是否可读
        if (stream.CanSeek)
        {
            // 重置流指针至起始位置
            stream.Position = 0;
        }

        // 初始化 StreamReader 实例
        using var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);

        // 读取完整内容
        _cachedRequestContent = await streamReader.ReadToEndAsync(cancellationToken);

        return _cachedRequestContent;
    }
}