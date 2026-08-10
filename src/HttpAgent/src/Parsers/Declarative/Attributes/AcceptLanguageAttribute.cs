// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式客户端所偏好的自然语言和区域特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class AcceptLanguageAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="AcceptLanguageAttribute" />
    /// </summary>
    /// <param name="language">自然语言和区域设置</param>
    public AcceptLanguageAttribute(string? language) => Language = language;

    /// <summary>
    ///     客户端偏好的语言和区域
    /// </summary>
    public string? Language { get; set; }
}