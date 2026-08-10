// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式请求 URL 编码表单内容特性
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FormUrlEncodedBodyAttribute : BodyAttribute
{
    /// <summary>
    ///     <inheritdoc cref="FormUrlEncodedBodyAttribute" />
    /// </summary>
    public FormUrlEncodedBodyAttribute()
        : base(MediaTypeNames.Application.FormUrlEncoded)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="FormUrlEncodedBodyAttribute" />
    /// </summary>
    /// <param name="contentEncoding">内容编码</param>
    public FormUrlEncodedBodyAttribute(string contentEncoding)
        : base(MediaTypeNames.Application.FormUrlEncoded, contentEncoding)
    {
    }
}