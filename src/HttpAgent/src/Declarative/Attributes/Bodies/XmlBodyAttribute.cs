// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求 XML 内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class XmlBodyAttribute : BodyAttribute
{
    /// <summary>
    ///     <inheritdoc cref="XmlBodyAttribute" />
    /// </summary>
    public XmlBodyAttribute()
        : base(MediaTypeNames.Text.Xml)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="XmlBodyAttribute" />
    /// </summary>
    /// <param name="contentEncoding">内容编码</param>
    public XmlBodyAttribute(string contentEncoding)
        : base(MediaTypeNames.Text.Xml, contentEncoding)
    {
    }
}