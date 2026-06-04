// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     字符串内容处理器
/// </summary>
public class StringContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is StringContent or JsonContent ||
        context.ContentType.IsIn([
            MediaTypeNames.Application.Json,
            MediaTypeNames.Application.JsonPatch,
            MediaTypeNames.Application.Xml,
            MediaTypeNames.Application.XmlPatch,
            MediaTypeNames.Text.Xml,
            MediaTypeNames.Text.Html,
            MediaTypeNames.Text.Plain,
            MediaTypeNames.Application.Soap // SOAP 1.2
        ], StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 将原始请求内容转换为字符串
        var content = context.RawContent!.GetType().IsBasicType() || context.RawContent is JsonElement or JsonNode
            ? context.RawContent.ToInvariantCultureString()
            : context.RawContent.ToJsonString(ResolveJsonSerializerOptions());

        // 初始化 StringContent 实例
        var stringContent = new StringContent(content!, context.Encoding,
            new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName });

        return stringContent;
    }
}