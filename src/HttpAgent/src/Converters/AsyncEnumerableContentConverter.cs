// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IAsyncEnumerable{T}" /> 内容转换器
/// </summary>
public class AsyncEnumerableContentConverter<T> : HttpContentConverterBase<IAsyncEnumerable<T?>>
{
    /// <inheritdoc />
    public override bool KeepsResponseAlive => true;

    /// <inheritdoc />
    public override IAsyncEnumerable<T?>? Read(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        // 获取 HttpResponseMessage 实例
        var httpResponseMessage = context.ResponseMessage;

        // 解析 HttpClient 客户端对应的 JSON 序列化上下文信息
        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(T), httpResponseMessage, ServiceProvider);

        return httpResponseMessage.Content.ReadFromJsonAsAsyncEnumerable<T>(
            jsonSerializationContext.JsonSerializerOptions, cancellationToken);
    }

    /// <inheritdoc />
    public override Task<IAsyncEnumerable<T?>?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default)
    {
        // 获取 HttpResponseMessage 实例
        var httpResponseMessage = context.ResponseMessage;

        // 解析 HttpClient 客户端对应的 JSON 序列化上下文信息
        var jsonSerializationContext =
            HttpRemoteUtility.ResolveJsonSerializationContext(typeof(T), httpResponseMessage, ServiceProvider);

        return Task.FromResult<IAsyncEnumerable<T?>?>(
            httpResponseMessage.Content.ReadFromJsonAsAsyncEnumerable<T>(jsonSerializationContext.JsonSerializerOptions,
                cancellationToken));
    }
}