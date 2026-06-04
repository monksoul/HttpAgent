// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IHttpContentProcessor" /> 内容处理器上下文
/// </summary>
/// <param name="RawContent">原始内容</param>
/// <param name="ContentType">内容类型</param>
/// <param name="Encoding">内容编码</param>
public sealed record HttpContentProcessorContext(object? RawContent, string ContentType, Encoding? Encoding = null)
{
    /// <summary>
    ///     <see cref="HttpClient" /> 实例的配置名称
    /// </summary>
    public string? HttpClientName { get; init; }
}