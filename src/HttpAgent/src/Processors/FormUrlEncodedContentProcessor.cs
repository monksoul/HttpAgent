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
    public override bool CanProcess(object? rawContent, string contentType) =>
        rawContent is FormUrlEncodedContent || contentType.IsIn([MediaTypeNames.Application.FormUrlEncoded],
            StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(object? rawContent, string contentType, Encoding? encoding)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(rawContent, contentType, encoding, out var httpContent))
        {
            return httpContent;
        }

        // 将原始请求类型转换为字符串字典类型
        var nameValueCollection = rawContent.ObjectToDictionary()!.ToDictionary(u => u.Key.ToInvariantCultureString()!,
            u => u.Value?.ToInvariantCultureString());

        // 初始化 FormUrlEncodedContent 实例
        var formUrlEncodedContent = new FormUrlEncodedContent(nameValueCollection);
        formUrlEncodedContent.Headers.ContentType =
            new MediaTypeHeaderValue(contentType) { CharSet = encoding?.BodyName };

        return formUrlEncodedContent;
    }
}