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
            .ReadFromJsonAsync(resultType, ResolveJsonSerializerOptions(httpResponseMessage), cancellationToken)
            .GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync(resultType,
            ResolveJsonSerializerOptions(httpResponseMessage), cancellationToken);

    /// <summary>
    ///     根据 HTTP 响应消息和服务提供器，解析出最终生效的 <see cref="JsonSerializerOptions" /> 实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <returns>
    ///     <see cref="JsonSerializerOptions" />
    /// </returns>
    protected virtual JsonSerializerOptions ResolveJsonSerializerOptions(HttpResponseMessage httpResponseMessage)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(httpResponseMessage);

        // 根据 HTTP 响应消息和服务提供器，解析出最终生效的 JsonSerializerOptions 实例
        return HttpRemoteUtility.ResolveJsonSerializerOptions(httpResponseMessage, ServiceProvider);
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
            .ReadFromJsonAsync<TResult>(ResolveJsonSerializerOptions(httpResponseMessage), cancellationToken)
            .GetAwaiter().GetResult();

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await httpResponseMessage.Content.ReadFromJsonAsync<TResult>(
            ResolveJsonSerializerOptions(httpResponseMessage), cancellationToken);
}