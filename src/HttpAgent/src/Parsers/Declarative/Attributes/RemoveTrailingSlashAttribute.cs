// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式移除 URL 地址末尾的 "/" 特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class RemoveTrailingSlashAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="RemoveTrailingSlashAttribute" />
    /// </summary>
    public RemoveTrailingSlashAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="RemoveTrailingSlashAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public RemoveTrailingSlashAttribute(bool enabled) => Enabled = enabled;

    /// <summary>
    ///     是否启用
    /// </summary>
    public bool Enabled { get; set; }
}