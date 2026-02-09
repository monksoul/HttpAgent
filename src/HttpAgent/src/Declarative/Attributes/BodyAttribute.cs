// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class BodyAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="BodyAttribute" />
    /// </summary>
    public BodyAttribute()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="BodyAttribute" />
    /// </summary>
    /// <param name="contentType">内容类型</param>
    public BodyAttribute(string contentType) => ContentType = contentType;

    /// <summary>
    ///     <inheritdoc cref="QueryAttribute" />
    /// </summary>
    /// <param name="contentType">内容类型</param>
    /// <param name="contentEncoding">内容编码</param>
    public BodyAttribute(string contentType, string contentEncoding)
        : this(contentType) =>
        ContentEncoding = contentEncoding;

    /// <summary>
    ///     内容类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    ///     内容编码
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    ///     是否使用 <see cref="StringContent" /> 构建 <see cref="FormUrlEncodedContent" />
    /// </summary>
    /// <remarks>当 <see cref="ContentType" /> 值为 <c>application/x-www-form-urlencoded</c> 时有效。默认值为：<c>fale</c>。</remarks>
    public bool UseStringContent { get; set; }

    /// <summary>
    ///     是否对表单数据进行 URL 编码
    /// </summary>
    /// <remarks>当 <see cref="ContentType" /> 值为 <c>application/x-www-form-urlencoded</c> 时有效。默认值为：<c>true</c>。</remarks>
    public bool UseUrlEncode { get; set; } = true;

    /// <summary>
    ///     是否为原始字符串内容
    /// </summary>
    /// <remarks>
    ///     <para>作用于 <see cref="string" /> 类型参数时有效。</para>
    ///     <para>
    ///         当属性值设置为 <c>true</c> 时，将校验 <see cref="ContentType" /> 属性值是否为空，并且字符串内容将被双引号包围并发送，格式如下：<c>"内容"</c>。默认值为：
    ///         <c>false</c>。
    ///     </para>
    /// </remarks>
    public bool RawString { get; set; }
}