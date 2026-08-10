// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式禁用 HTTP 缓存特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class DisableCacheAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="DisableCacheAttribute" />
    /// </summary>
    public DisableCacheAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="DisableCacheAttribute" />
    /// </summary>
    /// <param name="disabled">是否禁用</param>
    public DisableCacheAttribute(bool disabled) => Disabled = disabled;

    /// <summary>
    ///     是否禁用
    /// </summary>
    public bool Disabled { get; set; }
}