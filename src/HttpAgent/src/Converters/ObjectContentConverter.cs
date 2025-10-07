// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     对象内容转换器
/// </summary>
/// <remarks>默认作为 JSON 对象内容转换器。</remarks>
public class ObjectContentConverter : IHttpContentConverter
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content
            .ReadFromJsonAsync(resultType, GetJsonSerializerOptions(httpResponseMessage), cancellationToken)
            .GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync(resultType,
            GetJsonSerializerOptions(httpResponseMessage), cancellationToken);

    /// <summary>
    ///     获取 JSON 序列化选项实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="JsonSerializerOptions" />
    /// </returns>
    protected virtual JsonSerializerOptions GetJsonSerializerOptions(HttpResponseMessage httpResponseMessage)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 获取 HttpClient 实例的配置名称
        if (httpResponseMessage.RequestMessage?.Options.TryGetValue(
                new HttpRequestOptionsKey<string>(Constants.HTTP_CLIENT_NAME), out var httpClientName) != true)
        {
            httpClientName = string.Empty;
        }

        // 获取 HttpClientOptions 实例
        var httpClientOptions = ServiceProvider?.GetService<IOptionsMonitor<HttpClientOptions>>()?.Get(httpClientName);

        // 优先级：指定名称的 HttpClientOptions -> HttpRemoteOptions -> 默认值
        return (httpClientOptions?.IsDefault != false ? null : httpClientOptions.JsonSerializerOptions) ??
               ServiceProvider?.GetRequiredService<IOptions<HttpRemoteOptions>>().Value.JsonSerializerOptions ??
               HttpRemoteOptions.JsonSerializerOptionsDefault;
    }
}

/// <inheritdoc cref="ObjectContentConverter" />
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class ObjectContentConverter<TResult> : ObjectContentConverter, IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public virtual TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        httpResponseMessage.Content
            .ReadFromJsonAsync<TResult>(GetJsonSerializerOptions(httpResponseMessage), cancellationToken)
            .GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync<TResult>(
            GetJsonSerializerOptions(httpResponseMessage), cancellationToken);
}