// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式路径片段特性
/// </summary>
/// <remarks>支持多次指定。</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Parameter,
    AllowMultiple = true)]
public sealed class PathSegmentAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="PathSegmentAttribute" />
    /// </summary>
    /// <remarks>特性作用于参数时有效。</remarks>
    public PathSegmentAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="PathSegmentAttribute" />
    /// </summary>
    /// <remarks>
    ///     <para>当特性作用于方法或接口时，则表示添加指定路径片段操作。</para>
    ///     <para>当特性作用于参数且参数值为 <c>null</c> 时，表示将路径片段设置为 <c>segment</c> 的值。</para>
    /// </remarks>
    /// <param name="segment">路径片段</param>
    public PathSegmentAttribute(string segment)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        Segment = segment;
    }

    /// <summary>
    ///     路径片段
    /// </summary>
    public string? Segment { get; set; }

    /// <summary>
    ///     是否标记为待删除
    /// </summary>
    /// <remarks>默认为 <c>false</c>。设置为 <c>true</c> 表示移除路径片段。</remarks>
    public bool Remove { get; set; }
}