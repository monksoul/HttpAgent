// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     多部分表单内容数据内容处理器
/// </summary>
public class MultipartFormDataContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is MultipartFormDataContent ||
        context.ContentType.IsIn([MediaTypeNames.Multipart.FormData], StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context) =>
        // 尝试解析 HttpContent 类型
        TryProcess(context, out var httpContent)
            ? httpContent
            : throw new NotImplementedException();
}