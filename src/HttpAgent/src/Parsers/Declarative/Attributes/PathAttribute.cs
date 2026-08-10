// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式路径参数特性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
public sealed class PathAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="PathAttribute" />
    /// </summary>
    /// <param name="name">参数名称</param>
    /// <param name="value">参数值</param>
    public PathAttribute(string name, object? value)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Value = value;
    }

    /// <summary>
    ///     路径参数键
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     路径参数的值
    /// </summary>
    public object? Value { get; set; }
}