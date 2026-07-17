// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentConverter" /> 内容处理器上下文
/// </summary>
/// <param name="ResponseMessage">
///     <see cref="HttpResponseMessage" />
/// </param>
/// <param name="Converters"><see cref="IHttpContentConverter{TResult}" /> 集合</param>
public sealed record HttpContentConverterContext(
    HttpResponseMessage ResponseMessage,
    IReadOnlyList<IHttpContentConverter>? Converters = null)
{
    /// <summary>
    ///     请求耗时（毫秒）
    /// </summary>
    public long RequestDuration { get; init; }

    /// <inheritdoc cref="IHttpContentConverterFactory" />
    public IHttpContentConverterFactory Factory { get; init; } = null!;
}