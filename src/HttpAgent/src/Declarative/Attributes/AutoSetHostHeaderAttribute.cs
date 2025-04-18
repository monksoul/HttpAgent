﻿// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式设置自动 <c>Host</c> 标头特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
public sealed class AutoSetHostHeaderAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="AutoSetHostHeaderAttribute" />
    /// </summary>
    public AutoSetHostHeaderAttribute()
        : this(true)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="AutoSetHostHeaderAttribute" />
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public AutoSetHostHeaderAttribute(bool enabled) => Enabled = enabled;

    /// <summary>
    ///     是否启用
    /// </summary>
    public bool Enabled { get; set; }
}