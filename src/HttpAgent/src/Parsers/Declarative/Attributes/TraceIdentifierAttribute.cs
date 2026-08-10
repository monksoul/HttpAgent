// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式跟踪标识特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class TraceIdentifierAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="TraceIdentifierAttribute" />
    /// </summary>
    /// <param name="traceIdentifier">跟踪标识</param>
    public TraceIdentifierAttribute(string traceIdentifier) => Identifier = traceIdentifier;

    /// <summary>
    ///     跟踪标识
    /// </summary>
    public string Identifier { get; set; }
}