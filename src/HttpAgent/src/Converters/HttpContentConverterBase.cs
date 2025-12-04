// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentConverter{TResult}" /> 内容处理器基类
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public abstract class HttpContentConverterBase<TResult> : IHttpContentConverter<TResult>
{
    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public abstract TResult? Read(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<TResult?> ReadAsync(HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        Read(httpResponseMessage, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpResponseMessage httpResponseMessage,
        CancellationToken cancellationToken = default) =>
        await ReadAsync(httpResponseMessage, cancellationToken);

    /// <summary>
    ///     解析服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TService" />
    /// </returns>
    public TService? GetService<TService>() where TService : class => ServiceProvider?.GetService<TService>();
}