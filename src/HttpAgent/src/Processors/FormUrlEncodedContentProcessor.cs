// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     URL 编码的表单内容处理器
/// </summary>
public class FormUrlEncodedContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is FormUrlEncodedContent ||
        context.ContentType.IsIn([MediaTypeNames.Application.FormUrlEncoded], StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        var rawContent = context.RawContent;

        // 检查是否是字符串类型或字符串存储的 JsonNode 和 JsonElement
        if (rawContent is string ||
            (rawContent is JsonNode jsonNode && jsonNode.GetValueKind() == JsonValueKind.String) ||
            rawContent is JsonElement { ValueKind: JsonValueKind.String })
        {
            // 初始化 StringContent 实例（该方式不会自动编码，如空格）
            // 如需设置编码可注册 StringContentForFormUrlEncodedContentProcessor 处理器
            var stringContent = new StringContent(rawContent.ToInvariantCultureString()!, context.Encoding,
                new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName });

            return stringContent;
        }

        // 将原始请求类型转换为字符串字典类型
        var nameValueCollection = rawContent.ObjectToDictionary()!.ToDictionary(
            u => u.Key.ToInvariantCultureString()!,
            u => u.Value?.ToInvariantCultureString());

        // 初始化 FormUrlEncodedContent 实例
        var formUrlEncodedContent = new FormUrlEncodedContent(nameValueCollection);
        formUrlEncodedContent.Headers.ContentType =
            new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName };

        return formUrlEncodedContent;
    }
}