// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentConverter{TResult}" /> 工厂
/// </summary>
public interface IHttpContentConverterFactory
{
    /// <summary>
    ///     <inheritdoc cref="IServiceProvider" />
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为
    ///     <typeparamref name="TResult" />
    ///     实例
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <see cref="HttpContentConverterResult{TResult}" />
    /// </returns>
    HttpContentConverterResult<TResult> Read<TResult>(HttpContentConverterContext context,
        CancellationToken cancellationToken = default);

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
    ///     <see cref="HttpContentConverterResult" />
    /// </returns>
    HttpContentConverterResult Read(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为
    ///     <typeparamref name="TResult" />
    ///     实例
    /// </summary>
    /// <param name="context">
    ///     <see cref="HttpContentConverterContext" />
    /// </param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <see cref="HttpContentConverterResult{TResult}" />
    /// </returns>
    Task<HttpContentConverterResult<TResult>> ReadAsync<TResult>(HttpContentConverterContext context,
        CancellationToken cancellationToken = default);

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
    ///     <see cref="HttpContentConverterResult" />
    /// </returns>
    Task<HttpContentConverterResult> ReadAsync(Type resultType, HttpContentConverterContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     根据目标类型获取对应的 <see cref="IHttpContentConverter{TResult}" /> 实例
    /// </summary>
    /// <param name="context"><see cref="HttpContentConverterContext" /> 集合</param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <see cref="IHttpContentConverter{TResult}" />
    /// </returns>
    IHttpContentConverter<TResult> GetConverter<TResult>(HttpContentConverterContext context);

    /// <summary>
    ///     根据目标类型获取对应的 <see cref="IHttpContentConverter" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="context"><see cref="HttpContentConverterContext" /> 集合</param>
    /// <returns>
    ///     <see cref="IHttpContentConverter" />
    /// </returns>
    IHttpContentConverter GetConverter(Type resultType, HttpContentConverterContext context);
}