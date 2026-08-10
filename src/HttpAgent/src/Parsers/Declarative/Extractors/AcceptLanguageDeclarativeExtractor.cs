// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     HTTP 声明式 <see cref="AcceptLanguageAttribute" /> 特性提取器
/// </summary>
internal sealed class AcceptLanguageDeclarativeExtractor : IHttpDeclarativeExtractor
{
    /// <inheritdoc />
    public void Extract(HttpRequestBuilder httpRequestBuilder, HttpDeclarativeParsingContext context)
    {
        // 检查方法或接口是否贴有 [AcceptLanguage] 特性
        if (!context.IsMethodDefined<AcceptLanguageAttribute>(out var acceptLanguageAttribute, true))
        {
            return;
        }

        // 设置客户端所偏好的自然语言和区域设置
        httpRequestBuilder.AcceptLanguage(acceptLanguageAttribute.Language);
    }
}