// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求 HTML 内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class HtmlBodyAttribute : BodyAttribute
{
    /// <summary>
    ///     <inheritdoc cref="HtmlBodyAttribute" />
    /// </summary>
    public HtmlBodyAttribute()
        : base(MediaTypeNames.Text.Html)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="HtmlBodyAttribute" />
    /// </summary>
    /// <param name="contentEncoding">内容编码</param>
    public HtmlBodyAttribute(string contentEncoding)
        : base(MediaTypeNames.Text.Html, contentEncoding)
    {
    }
}