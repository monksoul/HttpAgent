// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentConverter{TResult}" /> 内容处理器基类
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public abstract class HttpContentConverterBase<TResult> : IHttpContentConverter<TResult>, IServiceProvider
{
    /// <inheritdoc />
    public virtual bool KeepsResponseAlive => false;

    /// <inheritdoc />
    public IServiceProvider? ServiceProvider { get; set; }

    /// <inheritdoc />
    public abstract TResult? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<TResult?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public virtual object? Read(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default) => Read(context, cancellationToken);

    /// <inheritdoc />
    public virtual async Task<object?> ReadAsync(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default) => await ReadAsync(context, cancellationToken);

    /// <inheritdoc />
    public object? GetService(Type serviceType) => ServiceProvider?.GetService(serviceType);
}