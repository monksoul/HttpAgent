// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式异常抑制特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class SuppressExceptionsAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="SuppressExceptionsAttribute" />
    /// </summary>
    /// <remarks>抑制所有异常。</remarks>
    public SuppressExceptionsAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="SuppressExceptionsAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用异常抑制。当设置为 <c>false</c> 时，将禁用异常抑制机制。</param>
    public SuppressExceptionsAttribute(bool enabled) => Types = enabled ? [typeof(Exception)] : [];

    /// <summary>
    ///     <inheritdoc cref="SuppressExceptionsAttribute" />
    /// </summary>
    /// <param name="types">异常抑制类型集合</param>
    public SuppressExceptionsAttribute(params Type[] types) => Types = types;

    /// <summary>
    ///     异常抑制类型集合
    /// </summary>
    public Type[] Types { get; set; }
}