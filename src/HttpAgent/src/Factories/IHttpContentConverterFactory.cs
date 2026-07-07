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
    ///     获取当前异步上下文中最后一次使用的 <see cref="IHttpContentConverter" />
    /// </summary>
    IHttpContentConverter? CurrentConverter { get; }

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为
    ///     <typeparamref name="TResult" />
    ///     实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    TResult? Read<TResult>(HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为 <see cref="object" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    object? Read(Type resultType, HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为
    ///     <typeparamref name="TResult" />
    ///     实例
    /// </summary>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <typeparam name="TResult">转换的目标类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TResult" />
    /// </returns>
    Task<TResult?> ReadAsync<TResult>(HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     将 <see cref="HttpResponseMessage" /> 转换为 <see cref="object" /> 实例
    /// </summary>
    /// <param name="resultType">转换的目标类型</param>
    /// <param name="httpResponseMessage">
    ///     <see cref="HttpResponseMessage" />
    /// </param>
    /// <param name="converters"><see cref="IHttpContentConverter{TResult}" /> 数组</param>
    /// <param name="cancellationToken">
    ///     <see cref="CancellationToken" />
    /// </param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    Task<object?> ReadAsync(Type resultType, HttpResponseMessage? httpResponseMessage,
        IHttpContentConverter[]? converters = null, CancellationToken cancellationToken = default);
}