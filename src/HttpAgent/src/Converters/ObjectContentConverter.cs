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
        CancellationToken cancellationToken = default)
    {
        // 根据 HTTP 响应消息和服务提供器，解析出 HttpClient 客户端对应的 JSON 响应反序列化时的上下文信息
        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(resultType, httpResponseMessage, ServiceProvider);

        // 获取 JSON 反序列化的值
        var deserializedValue = httpResponseMessage.Content.ReadFromJsonAsync(jsonSerializationContext.ResultType,
            jsonSerializationContext.JsonSerializerOptions, cancellationToken).GetAwaiter().GetResult();

        // 获取转换的目标类型值
        return jsonSerializationContext.GetResultValue(deserializedValue);
    }

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 根据 HTTP 响应消息和服务提供器，解析出 HttpClient 客户端对应的 JSON 响应反序列化时的上下文信息
        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(resultType, httpResponseMessage, ServiceProvider);

        // 获取 JSON 反序列化的值
        var deserializedValue = await httpResponseMessage.Content.ReadFromJsonAsync(jsonSerializationContext.ResultType,
            jsonSerializationContext.JsonSerializerOptions, cancellationToken);

        // 获取转换的目标类型值
        return jsonSerializationContext.GetResultValue(deserializedValue);
    }
}

/// <inheritdoc cref="ObjectContentConverter" />
/// <typeparam name="TResult">转换的目标类型</typeparam>
public class ObjectContentConverter<TResult> : ObjectContentConverter, IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public virtual TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        (TResult?)base.Read(typeof(TResult), httpResponseMessage, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        (TResult?)await base.ReadAsync(typeof(TResult), httpResponseMessage, cancellationToken);
}