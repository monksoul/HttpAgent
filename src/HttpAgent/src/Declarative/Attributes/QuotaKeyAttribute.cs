// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式配额键特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class QuotaKeyAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="QuotaKeyAttribute" />
    /// </summary>
    /// <param name="key">配额键</param>
    public QuotaKeyAttribute(string? key) => Key = key;

    /// <summary>
    ///     配额键
    /// </summary>
    /// <remarks>用于标识当前请求属于哪个配额组，例如接口路径。</remarks>
    public string? Key { get; set; }
}