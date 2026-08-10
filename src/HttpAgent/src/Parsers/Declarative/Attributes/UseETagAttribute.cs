// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式启用 ETag 缓存处理特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class UseETagAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="UseETagAttribute" />
    /// </summary>
    public UseETagAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="UseETagAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public UseETagAttribute(bool enabled) => Enabled = enabled;

    /// <summary>
    ///     是否启用
    /// </summary>
    public bool Enabled { get; set; }
}