// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 HTTP 版本特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class HttpVersionAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="HttpVersionAttribute" />
    /// </summary>
    /// <param name="version">HTTP 版本</param>
    public HttpVersionAttribute(string? version) => Version = version;

    /// <summary>
    ///     HTTP 版本
    /// </summary>
    public string? Version { get; set; }
}