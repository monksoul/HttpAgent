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
    public virtual bool KeepsResponseAlive => false;

    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        AsyncUtility.RunSync(() => ReadAsync(resultType, httpResponseMessage, cancellationToken));

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default)
    {
        // 解析 HttpClient 客户端对应的 JSON 序列化上下文信息
        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(resultType, httpResponseMessage, ServiceProvider);

        // 获取 JSON 反序列化的值
        var deserializedValue = !httpResponseMessage.ShouldJsonResponseStringUnwrap()
            ? await httpResponseMessage.Content.ReadFromJsonAsync(jsonSerializationContext.ResultType,
                jsonSerializationContext.JsonSerializerOptions, cancellationToken)
            // 解析经过双重序列化的 JSON 字符串，并将其反序列化为指定类型
            : await httpResponseMessage.Content.ReadAndUnwrapFromJsonAsync(jsonSerializationContext.ResultType,
                jsonSerializationContext.JsonSerializerOptions, cancellationToken);

        // 获取转换的目标类型值
        return jsonSerializationContext.GetResultValue(deserializedValue, httpResponseMessage);
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