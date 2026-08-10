// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式如果 HTTP 响应的 IsSuccessStatusCode 属性是 <c>false</c>，则引发异常特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class EnsureSuccessStatusCodeAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="EnsureSuccessStatusCodeAttribute" />
    /// </summary>
    public EnsureSuccessStatusCodeAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="EnsureSuccessStatusCodeAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public EnsureSuccessStatusCodeAttribute(bool enabled) => Enabled = enabled;

    /// <summary>
    ///     是否启用
    /// </summary>
    public bool Enabled { get; set; }
}