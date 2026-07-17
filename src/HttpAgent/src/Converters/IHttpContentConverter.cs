// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="HttpResponseMessage" /> 响应内容转换器
/// </summary>
public interface IHttpContentConverter
{
    /// <summary>
    ///     是否保持 <see cref="HttpResponseMessage" /> 存活
    /// </summary>
    bool KeepsResponseAlive { get; }

    /// <summary>
    ///     <inheritdoc cref="IServiceProvider" />
    /// </summary>
    IServiceProvider? ServiceProvider { get; set; }

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为 <see cref="object" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    object? Read(Type resultType, HttpContentConverterContext context, CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为 <see cref="object" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    Task<object?> ReadAsync(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
///     <see cref="HttpResponseMessage" /> 响应内容转换器
/// </summary>
/// <typeparam name="TResult">转换的目标类型</typeparam>
public interface IHttpContentConverter<TResult> : IHttpContentConverter
{
    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为目标类型
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    TResult? Read(HttpContentConverterContext context, CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为目标类型
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    Task<TResult?> ReadAsync(HttpContentConverterContext context,
        CancellationToken cancellationToken = default);
}