// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent;

/// <summary>
///     <see cref="IFormFile" /> 内容处理器
/// </summary>
public class FormFileContentProcessor : HttpContentProcessorBase
{
    /// <inheritdoc />
    public override bool CanProcess(HttpContentProcessorContext context) =>
        context.RawContent is IFormFile;

    /// <inheritdoc />
    public override HttpContent? Process(HttpContentProcessorContext context)
    {
        // 尝试解析 HttpContent 类型
        if (TryProcess(context, out var httpContent))
        {
            return httpContent;
        }

        // 获取 IFormFile 实例
        var formFile = (IFormFile)context.RawContent!;

        // 读取文件流（没有 using）
        var fileStream = formFile.OpenReadStream();

        // 添加请求结束后自动释放的流
        context.CompletionDisposable = fileStream;

        // 初始化 StreamContent 实例
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType =
            new MediaTypeHeaderValue(context.ContentType) { CharSet = context.Encoding?.WebName };

        // 设置请求内容 Content-Disposition 标头
        streamContent.Headers.ContentDisposition =
            new ContentDispositionHeaderValue(context.AsFormItem ? "form-data" : "attachment")
            {
                FileName = formFile.FileName
            };

        return streamContent;
    }
}