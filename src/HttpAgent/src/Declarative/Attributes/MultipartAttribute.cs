// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式多部分表单项内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class MultipartAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="MultipartAttribute" />
    /// </summary>
    public MultipartAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="BodyAttribute" />
    /// </summary>
    /// <param name="name">表单名称</param>
    public MultipartAttribute(string name) => Name = name;

    /// <summary>
    ///     表单名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     文件的名称
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    ///     表示将字符串作为多部分表单文件的来源
    /// </summary>
    /// <remarks>用于设置多部分表单文件内容。当参数类型为 <see cref="string" /> 时有效。</remarks>
    public FileSourceType AsFileFrom { get; set; }

    /// <summary>
    ///     表示是否作为表单的一项
    /// </summary>
    /// <remarks>
    ///     <para>当参数类型为对象类型时有效。</para>
    ///     <para>该属性值为 <c>true</c> 时作为表单的一项。否则将遍历对象类型的每一个公开属性作为表单的项。默认值为：<c>true</c>。</para>
    /// </remarks>
    public bool AsFormItem { get; set; } = true;
}